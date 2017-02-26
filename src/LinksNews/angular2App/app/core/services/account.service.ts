import { Http, Response, Request } from '@angular/http';
import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { RegistrationData } from '../domain/registration-data';
import { Account } from '../domain/account';
import { MessengerService } from "../../core/services/messenger.service";
import { TranslateService } from 'ng2-translate/ng2-translate';
import { Language } from "../domain/language";
import { UtilityService } from "../../core/services/utility.service";

@Injectable()
export class AccountService {

    languages: Array<Language>;

    constructor(
        public dataService: DataService, 
        public messengerService: MessengerService,
        public translate: TranslateService,
        public utilityService: UtilityService
        ) {
    }

    getLanguageByCode(languageCode: string): Language {
        if (!this.languages || !languageCode) {
            return undefined;
        }   
        for(let i: number = 0; i < this.languages.length; i++) {
            let language = this.languages[i];
            if (language.code && language.code.toLowerCase() === languageCode.toLowerCase()) {
                return language;
            }
        }
        return undefined;
    }

    getLanguageById(languageId: number): Language {
        if (!this.languages || !languageId) {
            return undefined;
        }   
        for(let i: number = 0; i < this.languages.length; i++) {
            let language = this.languages[i];
            if (language.code && language.id === languageId) {
                return language;
            }
        }
        return undefined;
    }

    public _account: Account;
    get account(): Account {
        if (!this.utilityService.getCookie("lp")) {
            this._account = undefined;
        }

        return this._account;
    }

    register(registration: RegistrationData, okCallback?: Function, errorCallback?: Function) {
        this.dataService.registerAccount(registration, 
                () => {
                    this.login(registration.login, registration.password, okCallback, errorCallback);
                },
                (error) => {
                    if (error && error.message) {
                        this.messengerService.showError("Error", error.message, () => {
                            if (errorCallback) {
                                errorCallback(error);
                            }
                        });
                        return;
                    }

                    if (errorCallback) {
                        errorCallback(error);
                    }
                }
            );
    }

    login(login: string, password: string, okCallback?: Function, errorCallback?: Function) {
        this.dataService.login(login, password, 
            (data) => {
                this._account = Account.from(data);
                this.setCurrentLanguage();
                if (okCallback) {
                    okCallback();
                }
            },
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message, () => {
                        if (errorCallback) {
                            errorCallback(error);
                        }
                    });
                    return;
                }

                if (errorCallback) {
                    errorCallback(error);
                }
            }
        );
    }

    logout(callback?: Function) {
        return this.dataService.logout(
            (data) => {
                this.setCurrentLanguage();
                if (callback) {
                    callback();
                }
            },
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }
            }
            );
    }

    get isAccountAuthenticated(): boolean {
        return this.account ? true : false;
    }

    setCurrentLanguage(languageCode?: string): void {

        if (!languageCode) {
            if (this.account && this.account.languageId) {
                let language: Language = this.getLanguageById(this.account.languageId);
                if (language) {
                    languageCode = language.code;
                }
            }
            languageCode = languageCode || this.utilityService.getCookie("language");
        }

        let language: Language = this.getLanguageByCode(languageCode);
        if (language) {
            this.translate.use(language.code);
            return;
        }

        let browserLanguage: string = this.translate.getBrowserLang();
        language = this.getLanguageByCode(browserLanguage);
        if (language) {
            this.translate.use(language.code);
            return;
        }
    
        this.translate.use("en");
    }

    getInterfaceLanguages() {
        this.dataService.getInterfaceLanguages(
            (data) => {
                this.languages = new Array<Language>();
                if (data) {
                    for (let i: number = 0; i < data.length; i++) {
                        let language: Language = Language.from(data[i]);
                        this.languages.push(language);
                    }
                }
                this.setCurrentLanguage();
            },
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }
            }
            );
    }

    refreshAccount(okCallback?: Function, errorCallback?: Function) {
        this.dataService.getAccount(
            (data) => {
                if (!data) {
                    this._account = undefined;
                    this.setCurrentLanguage();
                } else {
                    this._account = Account.from(data);
                    this.setCurrentLanguage();
                }
                
                if (okCallback) {
                    okCallback(data);
                }
           },
           (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message, () => {
                        if (errorCallback) {
                            errorCallback(error);
                        }
                    });
                    return;
                }

               if (errorCallback) {
                   errorCallback(error);
               }
           })
    }

    registerVisit(route: string) {
        let accountId: number = this.account ? this.account.id : undefined;
        this.dataService.registerVisit(route, accountId, 
            () => {},
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }
            }
            );
    }

}


