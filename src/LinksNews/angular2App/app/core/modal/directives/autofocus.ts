import { Directive, ElementRef } from '@angular/core';
import { ModalComponent } from '../components/modal';

@Directive({
    selector: '[autofocus]'
})
export class AutofocusDirective {
    constructor(public el: ElementRef, public modal: ModalComponent) {
        if (modal != null) {
            this.modal.onOpen.subscribe(() => {
                this.el.nativeElement.focus();
            });
        }
    }
}