import { Component, Input, ViewChild, OnInit, OnDestroy } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../../core/modal/components/modal';
import { MessengerService } from "../../../core/services/messenger.service";
import { DataService } from "../../../core/services/data.service";
import { Page } from "../../../core/domain/page/page";
import { PageComponent } from "../page.component";
import { Column } from "../../../core/domain/page/column";
import { Row } from "../../../core/domain/page/row";
import { ColumnTypes } from "../../../core/common/enums";
import { NewsSourceSelectorComponent } from "../../news/news-source-selector.component";
import { RowAction } from "../../../core/common/enums";
import { ColumnComponent } from "../column.component";
import { FileUploadComponent } from "./file-upload.component";
import { AccountService } from '../../../core/services/account.service';

@Component({
    selector: 'column-editor-placeholder',
    
    templateUrl: './column-editor.component.html'
})

export class ColumnEditorComponent implements OnInit, OnDestroy {

    @Input()
        columnComponent: ColumnComponent;


    @ViewChild('modalColumnEditor')
        columnEditor: any;

    @ViewChild('selectColumnType')
        selectColumnType: any;

    @ViewChild('uploader')
        uploader: FileUploadComponent;

    @ViewChild('newsSourcesPopup')
        newsSourcesPopup: NewsSourceSelectorComponent;
    
    newsPopupCreated: boolean = false;

    zIndex: number;
    column2edit: Column;
    maxHeight: number;


    @ViewChild('header') header;
    @ViewChild('footer') footer;

    constructor(
        public messengerService: MessengerService, 
        public dataService: DataService,
        public accountService: AccountService
        ) { 
        this.newsPopupCreated = false;
    }

    onShowNewsSourcesClick(){
        if (!this.newsPopupCreated) {
            this.newsPopupCreated = true;    
            return;
        }
        this.newsSourcesPopup.openPopup();
    }

    ngOnInit() {
    }

    ngOnDestroy() {
        this.newsPopupCreated = false;
    }

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

    get columnTypeId(): ColumnTypes {
        return this.column2edit.columnTypeId;
    }

    set columnTypeId(value: ColumnTypes) {
        if (this.column2edit.columnTypeId == value) {
            return;
        }
        this.column2edit.columnTypeId = value;
    }


    onEditorOpen() {
        this.messengerService.zIndex = this.messengerService.zIndex + 2;
        this.zIndex = this.messengerService.zIndex;
        jQuery("div.modal-backdrop").last().css("z-index", this.zIndex - 1);
        setTimeout(() => {
            this.setMaxHeight();
        },0);
    }

    openColumnEditor(columnComponent: ColumnComponent) {
        this.columnComponent = columnComponent;
        this.column2edit = this.columnComponent.column.clone();
        this.columnEditor.open('lg');
    }

    closeColumnEditor() {
        this.columnEditor.dismiss();
    }

    onEditorDismiss(){
    }

    saveColumn() {
        let rows2save = [];

        this.column2edit.rows.forEach(x => {
        
            if (!x.id) {
                rows2save.push({id: undefined, action: RowAction.Inserted });
                return;    
            }

            rows2save.push({id: x.id, action: RowAction.Unchanged });
        })

        this.columnComponent.column.rows.forEach(x => {
            if (!this.column2edit.findRowById(x.id)) {
                rows2save.push({id: x.id, action: RowAction.Deleted})
            }
        });

        let links2save = [];

        this.column2edit.links.forEach(x => {
        
            if (!x.id) {
                links2save.push({id: undefined, action: RowAction.Inserted });
                return;    
            }

            links2save.push({id: x.id, action: RowAction.Unchanged });
        })

        this.columnComponent.column.links.forEach(x => {
            if (x.newsLink) {
                return;
            }

            if (!this.column2edit.findLinkById(x.id)) {
                links2save.push({id: x.id, action: RowAction.Deleted})
            }
        });

        this.dataService.saveColumn(this.column2edit, rows2save, links2save,
                    (data) => {
                        this.columnEditor.dismiss().then(() => {
                                this.messengerService.showOK("Column save", "Changes have been saved", () => {
                                    this.columnComponent.refresh();
                                });
                                
                            }
                        );
                    },
                    (error) => {
                        if (error && error.message) {
                            this.messengerService.showError("Error", error.message);
                        }
                    }
        )
    }

    onShowTitleClick(){
        this.column2edit.showTitle = !this.column2edit.showTitle;
    }

    onShowImageClick(){
        this.column2edit.showImage = !this.column2edit.showImage;
    }

    onShowDescriptionClick(){
        this.column2edit.showDescription = !this.column2edit.showDescription;
    }

    onShowNewsImagesClick(){
        this.column2edit.showNewsImages = !this.column2edit.showNewsImages;
    }

    onShowNewsDescriptionsClick(){
        this.column2edit.showNewsDescriptions = !this.column2edit.showNewsDescriptions;
    }

    validate(): boolean {
        if (this.column2edit.columnTypeId == ColumnTypes.News && !this.column2edit.newsProviderSourceId) {
            this.messengerService.showWarning("Save column", "News Source is required");
            return false;
        }    
        return true;
    }

    saveColumnAndImage() {

        if (!this.validate()) {
            return;
        }

        if (!this.column2edit.imageUrl && !this.uploader.file) {
            this.dataService.deleteImage4Column(this.column2edit.id, 
                    () => {
                        this.saveColumn();
                    },
                    (error) => {
                        if (error && error.message) {
                            this.messengerService.showError("Error", error.message);
                        }
                    }
                );
            return;
        }

        let fileToUpload = this.uploader.file;

        if (fileToUpload) {
            
            this.dataService.uploadImage4Column(this.column2edit.id, fileToUpload,
                    () => {
                        this.saveColumn();
                    },
                    (error) => {
                        if (error && error.message) {
                            this.messengerService.showError("Error", error.message);
                        }
                    }
                );

            return;
        }

        this.saveColumn();
    }


}