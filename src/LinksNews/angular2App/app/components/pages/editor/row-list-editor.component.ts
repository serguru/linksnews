import { Component, Input, ViewChild, OnChanges, SimpleChanges, ViewChildren } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../../core/modal/components/modal';
import { MessengerService } from "../../../core/services/messenger.service";
import { DataService } from "../../../core/services/data.service";
import { Page } from "../../../core/domain/page/page";
import { PageComponent } from "../page.component";
import { Column } from "../../../core/domain/page/column";
import { Row } from "../../../core/domain/page/row";
import { RowEditorComponent } from "./row-editor.component";

@Component({
    selector: 'row-list-editor-placeholder',
    
    templateUrl: './row-list-editor.component.html'
})

export class RowListEditorComponent implements OnChanges {

    @Input()
        column: Column;

    //@ViewChildren("rowEditor")
    //    rowEditors: any;


    constructor(public messengerService: MessengerService, public dataService: DataService) { 
    }

    ngOnChanges(changes: SimpleChanges){
        if (!this.column || !this.column.rows || this.column.rows.length == 0) {
            return;
        }
        this.column.rows.forEach((x) => x.selected = x == this.column.rows[0]);
    }

    //onUpdateRowClick() {
    //    if (!this.rowEditors || this.rowEditors.length == 0) {
    //        return;
    //    }

    //    let selectedRow: Row = this.column.selectedRow;

    //    if (!selectedRow) {
    //        return;
    //    }

    //    for (let i: number = 0; i < this.rowEditors.length; i++) {

    //        let editor: RowEditorComponent = this.rowEditors._results[i];

    //        if (editor.row == selectedRow) {
    //            editor.openRowEditor();
    //            return;
    //        }
    //    }
    //}
}