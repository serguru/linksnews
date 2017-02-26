import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {AutofocusDirective} from '../directives/autofocus';
import {ModalFooterComponent} from '../components/modal-footer';
import {ModalBodyComponent} from '../components/modal-body';
import {ModalHeaderComponent} from '../components/modal-header';
import {ModalComponent} from '../components/modal';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        ModalComponent,
        ModalHeaderComponent,
        ModalBodyComponent,
        ModalFooterComponent,
        AutofocusDirective
    ],
    exports: [
        ModalComponent,
        ModalHeaderComponent,
        ModalBodyComponent,
        ModalFooterComponent,
        AutofocusDirective
    ]
})
export class ModalModule {
}
