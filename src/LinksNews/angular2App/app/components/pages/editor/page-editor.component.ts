import { Component, Input, ViewChild, OnChanges, SimpleChanges, AfterViewChecked } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../../core/modal/components/modal';
import { MessengerService } from "../../../core/services/messenger.service";
import { DataService } from "../../../core/services/data.service";
import { Page } from "../../../core/domain/page/page";
import { PageComponent } from "../page.component";
import { Row } from "../../../core/domain/page/row";
import { Column } from "../../../core/domain/page/column";
import { RowListEditorComponent } from "./row-list-editor.component";
import { RowAction } from "../../../core/common/enums";
//import * as _ from 'lodash';  

import { Router } from '@angular/router';
//import { ColumnEditorComponent } from "./column-editor.component";
import { AccountService } from "../../../core/services/account.service";
import { Account } from "../../../core/domain/account";
import { validationPattern } from "../../../core/common/constant";
import { FileUploadComponent } from "./file-upload.component";
import { ContentCategory } from "../../../core/domain/content-category";

@Component({
    selector: 'page-editor-placeholder',
    
    templateUrl: './page-editor.component.html',
})

export class PageEditorComponent  {

    row2edit: Row;
    page2edit: Page;
    pageComponent: PageComponent;
    zIndex: number;
    saveDisabled: boolean = true;
    pattern: any = validationPattern;
    maxHeight: number;

    @ViewChild('uploader')
        uploader: FileUploadComponent;

    constructor(
        public messengerService: MessengerService, 
        public dataService: DataService,
        public router: Router,
        public accountService: AccountService
        ) { 
    }

    @ViewChild('modalPageEditor')
        pageEditor: any;

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

    onCategoryClick(e, category: ContentCategory) {
        if (!e.target.checked) {
            category.selected = false;
            return;
        }
        if (this.page2edit.checkedCategoriesCount >= 3) {
            e.preventDefault();
            this.messengerService.showWarning("Warning", "Sorry but a page cannot belong to more than 3 categories");
            return;
        }    
        category.selected = true;
    }

    openPageEditor(pageComponent: PageComponent) {
        this.pageComponent = pageComponent;
        this.page2edit = pageComponent.page.cloneNoColumnsChildren();

        this.row2edit = new Row();
        this.row2edit.columnId = undefined;
        this.row2edit.columns = this.page2edit.columns;
        this.page2edit.columns = undefined;

        this.pageEditor.open('lg');
    }

    closePageEditor() {
        this.pageEditor.dismiss();
    }

    valid(): boolean {
        if (this.page2edit.publicAccess &&  this.page2edit.checkedCategoriesCount === 0) {
            this.messengerService.showWarning("Warning", "Published page must belong to at least one category");
            return false;

        }
        return true;
    }


    savePage() {

        let account: Account = this.accountService.account;

        if (!account || !account.login) {
            this.messengerService.closeWait(() => {
                this.messengerService.showWarning("Page saving", "You should be logged in to save a page");
            });
            return;
        }

        let columns2save = [];

        this.row2edit.columns.forEach(x => {
        
            if (!x.id) {
                columns2save.push({id: undefined, action: RowAction.Inserted, props: [x.columnWidth]});
                return;    
            }

            columns2save.push({id: x.id, action: RowAction.Unchanged, props: [x.columnWidth]});
        })

        this.pageComponent.page.columns.forEach(x => {
            if (!this.row2edit.findColumnById(x.id)) {
                columns2save.push({id: x.id, action: RowAction.Deleted})
            }
        });

        this.dataService.savePage(this.page2edit, columns2save,
            (data) => {
                this.messengerService.closeWait(() => {
                    this.pageEditor.dismiss().then(() => {
                            this.messengerService.showOK("Page save", "Changes have been saved", () => {
                                if (this.page2edit.name == this.pageComponent.pageName) {
                                    this.pageComponent.getPage(true);
                                } else {
                                    this.router.navigate(["/page", account.login, this.page2edit.name]);
                                }
                            });

                        }
                    );
                });
            },
            (error) => {
                this.messengerService.closeWait(() => {
                    if (error && error.message) {
                        this.messengerService.showError("Error", error.message);
                    }
                });
            }
        );
    }

    savePageAndImage() {

        if (!this.valid()){
            return;
        }

        this.messengerService.showWait("Saving a page...", () => {
            if (!this.page2edit.imageUrl && !this.uploader.file) {
                this.dataService.deleteImage4Page(this.page2edit.id, 
                        () => {
                            this.savePage();
                        },
                        (error) => {
                            this.messengerService.closeWait(() => {
                                if (error && error.message) {
                                    this.messengerService.showError("Error", error.message);
                                }
                            });
                        }
                );
                return;
            }

            let fileToUpload = this.uploader.file;

            if (fileToUpload) {
            
                this.dataService.uploadImage4Page(this.page2edit.id, fileToUpload,
                        () => {
                            this.savePage();
                        },
                        (error) => {
                            this.messengerService.closeWait(() => {
                                if (error && error.message) {
                                    this.messengerService.showError("Error", error.message);
                                }
                            });
                        }
                    );

                return;
            }

            this.savePage();
        });
    }
}