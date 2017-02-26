import { Component, Input, ViewChild, AfterViewChecked, ViewChildren, QueryList, OnDestroy } from '@angular/core';
import { PageComponent } from './page.component';
import { RowComponent } from './row.component';
import { Column } from '../../core/domain/page/column';
import { ColumnEditorComponent } from "./editor/column-editor.component";
import { DataService } from "../../core/services/data.service";
import { ColumnTypes } from "../../core/common/enums";
import { MessengerService } from "../../core/services/messenger.service";

@Component({
    selector: 'column-placeholder',
    
    templateUrl: './column.component.html'
})

export class ColumnComponent implements OnDestroy {

    constructor(
        public dataService: DataService,
        public messengerService: MessengerService, 
        ) { 
    }

    ngOnDestroy() {
    }

    @ViewChildren(RowComponent) rowComponents: QueryList<RowComponent>;

    @ViewChild("adPlaceholder") adPlaceholder;

    getPageComponent(): PageComponent {
        if (this.pageComponent) {
            return this.pageComponent;
        }

        if (this.rowComponent) {
            return this.rowComponent.getPageComponent();
        }
        
        return undefined;
    }


    get columnEditor(): ColumnEditorComponent {
        let pageComponent: PageComponent = this.getPageComponent();

        if (!pageComponent) {
            return undefined;
        }

        return pageComponent.columnEditor;
    }

    @Input() 
        column: Column;

    @Input() 
        pageComponent: PageComponent;

    @Input() 
        rowComponent: RowComponent;


    openColumnEditor() {
        this.columnEditor.openColumnEditor(this);
    }


    refreshNews() {

        if (!this.column) {
            return;
        }

        if (this.column.columnTypeId == ColumnTypes.News) {
            this.refresh();
            return;
        }

        if (this.column.columnTypeId != ColumnTypes.Rows || !this.rowComponents) {
            return;
        }

        this.rowComponents.forEach((x) => {
            x.refreshNews();
        })
    }


    get editMode(): boolean {

        if (this.rowComponent) {
            return this.rowComponent.editMode;
        }

        if (this.pageComponent) {
            return this.pageComponent.editMode;
        }

        return false;
    }

    refresh() {
        if (!this.column || !this.column.id) {
            return;
        }

        this.dataService.getColumnById(this.column.id,
                    (data) => {
                        let oldColumn: Column = this.column;
                        this.column = Column.from(data);

                        if (this.rowComponent) {
                            let index: number = this.rowComponent.row.columns.indexOf(oldColumn);
                            this.rowComponent.row.columns.splice(index,1);
                            this.rowComponent.row.columns.splice(index,0,this.column);
                            return;
                        }

                        if (this.pageComponent) {
                            let index: number = this.pageComponent.page.columns.indexOf(oldColumn);
                            this.pageComponent.page.columns.splice(index,1);
                            this.pageComponent.page.columns.splice(index,0,this.column);
                        }
                    },
                    (error) => {
                        this.messengerService.closeWait(() => {
                            if (error && error.message) {
                                this.messengerService.showError("Error", error.message);
                            }
                        });
                    }
            )
    }

}
