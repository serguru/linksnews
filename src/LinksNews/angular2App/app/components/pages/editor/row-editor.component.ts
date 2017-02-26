import { Component, Input, ViewChild } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../../core/modal/components/modal';
import { MessengerService } from "../../../core/services/messenger.service";
import { DataService } from "../../../core/services/data.service";
import { Page } from "../../../core/domain/page/page";
import { PageComponent } from "../page.component";
import { Column } from "../../../core/domain/page/column";
import { Row } from "../../../core/domain/page/row";
import { RowComponent } from "../row.component";
import { RowAction } from "../../../core/common/enums";
import { AccountService } from '../../../core/services/account.service';

@Component({
    selector: 'row-editor-placeholder',
    
    templateUrl: './row-editor.component.html'
})

export class RowEditorComponent {

    @ViewChild('modalRowEditor') rowEditor: any;

    rowComponent: RowComponent;
    
    zIndex: number;
    row2edit: Row;
    maxHeight: number;

    constructor(
        public messengerService: MessengerService, 
        public dataService: DataService,
        public accountService: AccountService
        ) { 
    }

    @ViewChild('header') header;
    @ViewChild('footer') footer;

    get headerHeight(): number {
        return this.header ? this.header.nativeElement.offsetHeight : 30;
    }

    get footerHeight(): number {
        return this.footer ? this.footer.nativeElement.offsetHeight : 38;
    }

    setMaxHeight() {
        let maxHeight: number =  window.innerHeight - this.headerHeight - this.footerHeight - 150;

        if (this.maxHeight === maxHeight) {
            return;
        }
        this.maxHeight = maxHeight;
    }

    onWindowResize() {
        this.setMaxHeight();
    }

    onEditorOpen() {
        this.messengerService.zIndex = this.messengerService.zIndex + 2;
        this.zIndex = this.messengerService.zIndex;
        jQuery("div.modal-backdrop").last().css("z-index", this.zIndex - 1);
        setTimeout(() => {
            this.setMaxHeight();
        },0);
    }

    openRowEditor(rowComponent: RowComponent) {
        this.rowComponent = rowComponent;
        this.row2edit = this.rowComponent.row.clone();
        this.rowEditor.open('lg');
    }

    closeRowEditor() {
        this.rowEditor.dismiss();
    }

    onEditorDismiss(){
    }

    saveRow() {
        // TODO: check for changes
        let columns2save = [];

        this.row2edit.columns.forEach(x => {
        
            if (!x.id) {
                columns2save.push({id: undefined, action: RowAction.Inserted, props: [x.columnWidth]});
                return;    
            }

            columns2save.push({id: x.id, action: RowAction.Unchanged, props: [x.columnWidth]});
        });

        this.rowComponent.row.columns.forEach(x => {
            if (!this.row2edit.findColumnById(x.id)) {
                columns2save.push({id: x.id, action: RowAction.Deleted})
            }
        });

        this.dataService.saveRow(this.row2edit, columns2save,
            (data) => {
                this.rowEditor.dismiss().then(() => {
                        this.messengerService.showOK("Row save", "Changes have been saved", () => {
                            this.rowComponent.refresh();
                        });
                    }
                );
            },
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }
            }
        );
    }
}