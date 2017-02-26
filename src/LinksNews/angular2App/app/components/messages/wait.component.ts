import { Component, ViewChild } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../core/modal/components/modal';
import { MessageConfig } from "../../core/domain/message-config";
import { DialogButtonComponent } from "./dialog-button.component";
import { DialogButtonConfig } from "../../core/domain/dialog-button-config";
import { DialogButtonType } from "../../core/common/enums";
import { MessengerService } from "../../core/services/messenger.service";

@Component({
    selector: 'wait-placeholder',
    
    templateUrl: './wait.component.html'
})

export class WaitComponent {

    zIndex: number;

    public _message: string;

    get message(): string {
        return this._message ? this._message : "Working..."
    }

    set message(value: string) {
        if (this._message !== value) {
            this._message = value;
        }
    }


    @ViewChild("modalDialog")
        dialog: any;

    constructor(public messengerService: MessengerService) { 
    }

    show(message?: string, afterOpenCallback?: Function ){
        this.message = message;
        this.dialog.open('sm').then(() => {
            if (afterOpenCallback) {
                afterOpenCallback();
            }
        });
    }

    close(afterCloseCallback?: Function) {
        this.dialog.close().then(() => {
            if (afterCloseCallback) {
                afterCloseCallback();
            }
        });
    }

    onDialogOpen() {
        this.messengerService.zIndex = this.messengerService.zIndex + 2;
        this.zIndex = this.messengerService.zIndex;
        jQuery("div.modal-backdrop").last().css("z-index", this.zIndex - 1);
    }

    public checkModalOpen(){
        let visibleModals = jQuery("modal:visible");
        if (visibleModals.length > 0)
        {
            jQuery("body").addClass("modal-open");
        }
    }

    onDialogDismiss(){
        this.checkModalOpen();
    }

    onDialogClose(){
        this.checkModalOpen();
    }
}
