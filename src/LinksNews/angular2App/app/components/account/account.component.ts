import { Component, OnInit, ViewChild } from '@angular/core';
import { Page } from "../../core/domain/page/page";
import { Router, ActivatedRoute } from '@angular/router';
import { DataService } from '../../core/services/data.service';
import { Account } from "../../core/domain/account";
import { MessengerService } from '../../core/services/messenger.service';
//import * as _ from 'lodash';
import { validationPattern } from "../../core/common/constant";
import { FileUploadComponent } from "../pages/editor/file-upload.component";
import { AccountService } from "../../core/services/account.service";
import { Language } from "../../core/domain/language";

@Component({
    
    templateUrl: './account.component.html',
})

export class AccountComponent implements OnInit {
    account: Account;
    accountUntouched: Account;
    pattern: any = validationPattern;
    @ViewChild('uploader')
    uploader: FileUploadComponent;

    @ViewChild('passwordEditor') passwordEditor;

    newsRefreshIntervals = [
        { value: null, text: "Never" },
        { value: 10, text: "10 minutes" },
        { value: 30, text: "30 minutes" },
        { value: 60, text: "1 hour" },
        { value: 180, text: "3 hours" },
        { value: 360, text: "6 hours" }
    ]

    constructor(
        public route: ActivatedRoute,
        public dataService: DataService,
        public router: Router,
        public messengerService: MessengerService,
        public accountService: AccountService
    ) {
    }

    get title(): string {
        return "account";
    }

    get languages(): Array<Language> {
        return this.accountService.languages ? this.accountService.languages : new Array<Language>();
    }

    get language(): Language {
        if (!this.account || !this.account.languageId || !this.accountService.languages) {
            return undefined;
        }

        for (let i: number = 0; i < this.accountService.languages.length; i++) {
            if (this.accountService.languages[i].id == this.account.languageId) {
                return this.accountService.languages[i];
            }
        }
        return undefined;
    }

    reject() {
        this.messengerService.showOK("Login required", "Please log in to have accsess to your account", () => {
            this.router.navigate(["/login"])
        });
    }

    ngOnInit() {
        this.accountService.refreshAccount(() => {
            if (!this.accountService.account) {
                this.reject();
                return;
            }

            this.account = this.accountService.account.clone();
            this.accountUntouched = this.accountService.account.clone();
        },

            (errorCode) => {
                if (errorCode === 401) {
                    this.reject();
                }
            });
    }

    saveAccount() {
        if (<any>this.account.newsRefreshInterval == "null") {
            this.account.newsRefreshInterval = null;
        }

        this.dataService.saveAccount(this.account,
            () => {
                this.accountService.refreshAccount(() => {
                    this.messengerService.showOK("Save account", "Changes have been saved", () => {
                        this.ngOnInit();
                    });
                });
            },
            (error) => {
                if (error && error.code === 401) {
                    this.reject();
                }
            }
        );
    }

    validate(): boolean {
        let account: Account = this.accountService.account;

        if (!account || !account.login) {
            this.messengerService.showWarning("Warning", "You should be logged in");
            return false;
        }

        if (this.account.login != account.login) {
            this.messengerService.showWarning("Warning", "Account does not match logged in account");
            return false;
        }

        return true;
    }

    saveAccountAndImage() {
        if (!this.validate()) {
            return;
        }

        if (!this.account.imageUrl && !this.uploader.file) {
            this.dataService.deleteImage4Account(this.account.login,
                () => {
                    this.saveAccount();
                },
                (error) => {
                    if (error && error.message) {
                        this.messengerService.showError("Error", error.message);
                    }
                }
            );
            return;
        }

        let fileToUpload = this.uploader.file;

        if (fileToUpload) {
            this.dataService.uploadImage4Account(this.account.login, fileToUpload,
                () => {
                    this.saveAccount();
                },
                (error) => {
                    if (error && error.message) {
                        this.messengerService.showError("Error", error.message);
                    }
                }
            );

            return;
        }

        this.saveAccount();
    }

    cancel() {
        this.account = this.accountUntouched.clone();
    }

    delete() {
        if (!this.validate()) {
            return;
        }

        this.messengerService.showOkCancel("Delete account", "Are you sure you want to delete your account?",
            () => {
                this.messengerService.showOkCancel("Delete account confirm",
                    "All your pages and account data will be deleted. These data cannot be restored. Delete account?",
                    () => {
                        this.dataService.deleteAccount(this.account.login,
                            () => {
                                this.messengerService.showOK("Delete account", "Your account has been deleted", () => {
                                    this.accountService.setCurrentLanguage();
                                    this.router.navigate(["/home"]);
                                });
                            },
                            (error) => {
                                if (error && error.message) {
                                    this.messengerService.showError("Error", error.message);
                                }
                            }

                        );
                    });
            }

        );
    }
}