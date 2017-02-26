import { Component, ViewChild } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../core/modal/components/modal';
import { MessageConfig } from "../../core/domain/message-config";
import { DialogButtonComponent } from "./dialog-button.component";
import { DialogButtonConfig } from "../../core/domain/dialog-button-config";
import { DialogButtonType } from "../../core/common/enums";
import { MessengerService } from "../../core/services/messenger.service";

@Component({
    selector: 'messenger-placeholder',
    
    templateUrl: './messenger.component.html'
})

export class MessengerComponent {

    config: MessageConfig;

    zIndex: number;

    @ViewChild("modalDialog")
        dialog: ModalComponent;

    constructor(public messengerService: MessengerService) { 
    }

    //show(config: MessageConfig, callback?: Function){
    //    this.config = config;
    //    this.dialog.open().then(() => {
    //        if (callback) {
    //            callback();
    //        }
    //    });
    //}

    onDialogOpen() {
        this.messengerService.zIndex = this.messengerService.zIndex + 2;
        this.zIndex = this.messengerService.zIndex;
        jQuery("div.modal-backdrop").last().css("z-index", this.zIndex - 1);
    }

    showOK(header: string, message: string, callback?: Function) {
        let config: MessageConfig = new MessageConfig(message, header, "", 
            [new DialogButtonConfig(DialogButtonType.OK, callback)]);

        this.config = config;
        this.dialog.open();
    }

    showOkCancel(title: string, message: string, onOk: Function, onCancel?: Function){
        let config: MessageConfig = new MessageConfig(message, title, "", 
            [
                new DialogButtonConfig(DialogButtonType.OK, onOk), 
                new DialogButtonConfig(DialogButtonType.Cancel, onCancel)
            ]);

        this.config = config;
        this.dialog.open();
    }

    showError(title: string, message: string, callback?: Function) {
        let config: MessageConfig = new MessageConfig(message, title, "", 
            [new DialogButtonConfig(DialogButtonType.OK, callback)]);

        this.config = config;
        this.dialog.open();
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
