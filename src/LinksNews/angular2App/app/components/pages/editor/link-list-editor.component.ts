import { Component, Input, ViewChild, ViewChildren, OnChanges, SimpleChanges } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../../core/modal/components/modal';
import { MessengerService } from "../../../core/services/messenger.service";
import { DataService } from "../../../core/services/data.service";
import { Page } from "../../../core/domain/page/page";
import { PageComponent } from "../page.component";
import { Column } from "../../../core/domain/page/column";
import { Link } from "../../../core/domain/page/link";
import { LinkEditorComponent } from "./link-editor.component";

@Component({
    selector: 'link-list-editor-placeholder',
    
    templateUrl: './link-list-editor.component.html'
})

export class LinkListEditorComponent implements OnChanges {

    @Input()
        column: Column;

    @ViewChildren("linkEditor")
        linkEditors: any;


    constructor(public messengerService: MessengerService, public dataService: DataService) { 
    }

    ngOnChanges(changes: SimpleChanges){
        if (!this.column || !this.column.links || this.column.links.length == 0) {
            return;
        }
        this.column.links.forEach((x) => x.selected = x == this.column.links[0]);
    }

}