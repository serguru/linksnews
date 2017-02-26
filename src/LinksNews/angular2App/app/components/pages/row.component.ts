import { Component, Input, ViewChild, AfterViewChecked, ViewChildren, QueryList } from '@angular/core';
import { Column } from '../../core/domain/page/column';
import { Row } from '../../core/domain/page/row';
import { ColumnComponent } from './column.component';
import { DataService } from "../../core/services/data.service";
import { RowEditorComponent } from './editor/row-editor.component';
import { PageComponent } from './page.component';
import { MessengerService } from "../../core/services/messenger.service";

@Component({
    selector: 'row-placeholder',
    
    templateUrl: './row.component.html'
})

export class RowComponent {

    constructor(
        public dataService: DataService,
        public messengerService: MessengerService 
        ) { 
    }

    @Input() columnComponent: ColumnComponent;
    @Input() row: Row;

    @ViewChildren('columnPlaceholder') columnComponents: QueryList<ColumnComponent>;

    @ViewChild("adPlaceholder") adPlaceholder;

    get rowEditor(): RowEditorComponent {

        let pageComponent: PageComponent = this.getPageComponent();

        if (!pageComponent) {
            return undefined;
        }

        return pageComponent.rowEditor;
    }

    getPageComponent(): PageComponent {
        if (this.columnComponent) {
            return this.columnComponent.getPageComponent();
        }
        return undefined;
    }

    openRowEditor() {
        if (!this.rowEditor) {
            return;
        }
        this.rowEditor.openRowEditor(this);
    }

    refreshNews() {
        if (!this.columnComponents) {
            return;
        }

        this.columnComponents.forEach((x) => {
            x.refreshNews();
        })
    }


    get editMode(): boolean {
        return this.columnComponent.editMode;
    }

    refresh() {
        if (!this.row || !this.row.id) {
            return;
        }

        this.dataService.getRowById(this.row.id,
                    (data) => {

                        let oldRow: Row = this.row;
                        this.row = Row.from(data);

                        if (!this.columnComponent) {
                            return;
                        }
                        let index: number = this.columnComponent.column.rows.indexOf(oldRow);
                        this.columnComponent.column.rows.splice(index,1);
                        this.columnComponent.column.rows.splice(index,0,this.row);
                    },
                    (error) => {
                        if (error && error.message) {
                            this.messengerService.showError("Error", error.message);
                        }
                    }
        )
    }
}
