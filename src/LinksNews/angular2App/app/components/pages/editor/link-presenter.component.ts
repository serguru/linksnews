import { Component, Input, ViewChild } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../../core/modal/components/modal';
import { MessengerService } from "../../../core/services/messenger.service";
import { DataService } from "../../../core/services/data.service";
import { Page } from "../../../core/domain/page/page";
import { PageComponent } from "../page.component";
import { Link } from "../../../core/domain/page/link";
import { Column } from "../../../core/domain/page/column";

@Component({
    selector: 'link-presenter-placeholder',
    
    templateUrl: './link-presenter.component.html'
})

export class LinkPresenterComponent {

    @Input()
        column: Column;

    @Input()
        link: Link;

    constructor(public messengerService: MessengerService, public dataService: DataService) { 
    }
}