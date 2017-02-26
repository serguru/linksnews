import { Component, Input } from '@angular/core';
import { DialogButtonConfig } from "../../core/domain/dialog-button-config";
import { MessengerComponent } from "./messenger.component";
import { DialogButtonType } from "../../core/common/enums";

@Component({
    selector: 'dialog-button-placeholder',
    
    templateUrl: './dialog-button.component.html'
})

export class DialogButtonComponent {
    @Input()
    config: DialogButtonConfig;

    @Input()
    parent: MessengerComponent;

    convertButtonTypeToCaption(type: DialogButtonType): string {
        return DialogButtonType[type];    
    }
    
    onClick() {
        this.parent.dialog.close().then(() => {
            if (this.config.handler) {
                this.config.handler();
            }
        })
    }
}


