import { Injectable } from '@angular/core';
import { MessageConfig } from "../domain/message-config";
import { MessengerComponent } from "../../components/messages/messenger.component";
import { WaitComponent } from "../../components/messages/wait.component";

@Injectable()
export class MessengerService {


    zIndex: number = 2000;

    messenger: MessengerComponent;
    wait: WaitComponent;

    constructor () {
    }

    showOK(title: string, message: string, callback?: Function){
        this.messenger.showOK(title, message, callback);
    }

    showNotImplemented(){
        this.messenger.showOK("Info", "Sorry but this feature is not implemented yet");
    }

    showWarning(title: string, message: string, callback?: Function){
        this.messenger.showOK(title, message, callback);
    }

    showError(title: string, message: string, callback?: Function) {
        this.messenger.showError(title, message, callback);
    }

    showOkCancel(title: string, message: string, onOk: Function, onCancel?: Function){
        this.messenger.showOkCancel(title, message, onOk, onCancel);
    }

    showWait(message?: string, afterOpenCallback?: Function) {
        this.wait.show(message, afterOpenCallback);
    }

    closeWait(afterCloseCallback?: Function) {
        this.wait.close(afterCloseCallback);
    }

}