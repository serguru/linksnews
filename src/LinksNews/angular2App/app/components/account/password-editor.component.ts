import { Component, Input, ViewChild } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../core/modal/components/modal';
import { MessengerService } from "../../core/services/messenger.service";
import { DataService } from "../../core/services/data.service";
import { AccountComponent } from "./account.component";
import { AccountService } from "../../core/services/account.service";
import { validationPattern } from "../../core/common/constant";

@Component({
    
    selector: 'password-editor-placeholder',
    templateUrl: './password-editor.component.html'
})

export class PasswordEditorComponent {

    pattern: any = validationPattern;


    @ViewChild('modalPasswordEditor')
        modalPasswordEditor: any;

    zIndex: number;
    
    oldPassword: string;
    newPassword: string;
    passwordConfirm: string;

    constructor(public messengerService: MessengerService, 
        public dataService: DataService,
        public accountService: AccountService
        ) { 
    }

    onEditorOpen() {
        this.messengerService.zIndex = this.messengerService.zIndex + 2;
        this.zIndex = this.messengerService.zIndex;
        jQuery("div.modal-backdrop").last().css("z-index", this.zIndex - 1);
    }

    openPasswordEditor() {
        this.oldPassword = undefined;
        this.newPassword = undefined;
        this.passwordConfirm = undefined;
        this.modalPasswordEditor.open('sm');
    }

    close() {
        this.modalPasswordEditor.dismiss();
    }

    onEditorDismiss(){
//        jQuery("body").addClass("modal-open");
    }

    valid(): boolean {
        if (this.oldPassword === this.newPassword) {
            this.messengerService.showWarning("Warning", "Passwords are the same");
            return false;
        }

        if (this.newPassword !== this.passwordConfirm) {
            this.messengerService.showWarning("Warning", "New password does not match password confirmation");
            return false;
        }

        return true;
    }

    save() {

        if (!this.valid()) {
            return;
        }

        this.messengerService.showWait("Changing password...", () => {
            this.dataService.savePassword(this.oldPassword, this.newPassword, this.accountService.account.login,
                        () => {
                            this.messengerService.closeWait(() => {
                                this.modalPasswordEditor.dismiss().then(() => {
                                        this.messengerService.showOK("Password save", "Password has been changed");
                                    }
                                );
                            });
                        },
                        (error) => {
                            this.messengerService.closeWait(() => {
                                if (error && error.message) {
                                    this.messengerService.showError("Error", error.message);
                                }
                            });
                        }
                    );
        });
    }
}