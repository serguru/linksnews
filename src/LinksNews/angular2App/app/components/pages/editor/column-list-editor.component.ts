import { Component, Input, ViewChild, OnChanges, SimpleChanges, ViewChildren } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../../core/modal/components/modal';
import { MessengerService } from "../../../core/services/messenger.service";
import { DataService } from "../../../core/services/data.service";
import { Page } from "../../../core/domain/page/page";
import { PageComponent } from "../page.component";
import { Column } from "../../../core/domain/page/column";
import { Row } from "../../../core/domain/page/row";
//import { ColumnEditorComponent } from "./column-editor.component";


@Component({
    selector: 'column-list-editor-placeholder',
    
    templateUrl: './column-list-editor.component.html'
})

export class ColumnListEditorComponent implements OnChanges {

    @Input()
        row: Row;

    constructor(public messengerService: MessengerService, public dataService: DataService) { 
    }


    addColumn() {

        if (this.row.columns.length >= 12) {
            this.messengerService.showOK("Add column", "A page cannot have more than 12 columns");
            return;
        }

        if (this.row.totalColumnsWidth >= 12) {
            this.messengerService.showOK("Add column", 
                `All available space is occupied. 
                To add a new column please make narrower one of the existing columns.
                `);
            return;
        }
        this.row.addNewColumn();
    }

    ngOnChanges(changes: SimpleChanges){
        if (!this.row || !this.row.columns || this.row.columns.length == 0) {
            return;
        }
        this.row.columns.forEach((x) => x.selected = x == this.row.columns[0]);
    }

}