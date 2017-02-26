import { Component, Input, ViewChild } from '@angular/core';
//import { ModalComponent } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalComponent } from '../../../core/modal/components/modal';
import { MessengerService } from "../../../core/services/messenger.service";
import { DataService } from "../../../core/services/data.service";
import { Page } from "../../../core/domain/page/page";
import { Row } from "../../../core/domain/page/row";
import { Column } from "../../../core/domain/page/column";
import { RowListEditorComponent } from "./row-list-editor.component";
import { RowAction } from "../../../core/common/enums";
import { MyPagesComponent } from "../my-pages.component";
import { AccountService } from "../../../core/services/account.service";
import { Account } from "../../../core/domain/account";

//import * as _ from 'lodash';  

import { Router } from '@angular/router';


@Component({
    selector: 'my-pages-editor-placeholder',
    
    templateUrl: './my-pages-editor.component.html',
})

export class MyPagesEditorComponent {

    pages2edit: Array<Page>;

    @ViewChild('modalPagesListEditor')
        pagesListEditor: any;

    myPagesComponent: MyPagesComponent;

    zIndex: number;
    saveDisabled: boolean = true;
    maxHeight: number;

    constructor(
        public messengerService: MessengerService, 
        public dataService: DataService,
        public router: Router,
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


    openEditor(myPagesComponent: MyPagesComponent) {
        this.myPagesComponent = myPagesComponent;
        this.pages2edit = new Array<Page>();
        if (myPagesComponent.pages) {
            myPagesComponent.pages.forEach(x => this.pages2edit.push(x.clone()));
        }
        if (this.pages2edit.length > 0) {
            this.setPageSelected(this.pages2edit[0]);
        }
        this.pagesListEditor.open('lg');
    }

    onEditorOpen() {
        this.messengerService.zIndex = this.messengerService.zIndex + 2;
        this.zIndex = this.messengerService.zIndex;
        jQuery("div.modal-backdrop").last().css("z-index", this.zIndex - 1);
        setTimeout(() => {
            this.setMaxHeight();
        },0);
    }

    onEditorDismiss() {
        this.myPagesComponent.editMode = false;
    }

    dismissPagesListEditor() {
        this.pagesListEditor.dismiss();
    }

    findPageById(id: number): Page {
        if (!this.pages2edit) {
            return undefined;
        }
        for (let i: number = 0; i < this.pages2edit.length; i++) {
            let page: Page = this.pages2edit[i];
            if (page.id == id) {
                return page;
            }
        }
        return undefined;
    }


    savePagesList() {

        this.messengerService.showWait("Saving changes...", () => {
            let pages2save = [];

            this.pages2edit.forEach(x => {
        
                if (!x.id) {
                    pages2save.push({id: undefined, action: RowAction.Inserted});
                    return;    
                }

                pages2save.push({id: x.id, action: RowAction.Unchanged});
            })

            let account: Account = this.accountService.account;

            this.myPagesComponent.pages.forEach(x => {
                if (!this.findPageById(x.id)) {
                    pages2save.push({id: x.id, action: RowAction.Deleted});
                    if (account) {
                        this.dataService.removePageFromCache(account.login, x.name, 
                            () => {},
                            (error) => {
                                if (error && error.message) {
                                    this.messengerService.showError("Error", error.message);
                                }
                            }
                            );
                    }
                }
            });

            this.dataService.savePagesList(pages2save,
                        (data) => {
                            this.messengerService.closeWait(()=> {
                                this.pagesListEditor.dismiss().then(() => {
                                        this.messengerService.showOK("Pages list save", "Changes have been saved", () => {
                                            this.myPagesComponent.ngOnInit();
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
        
        });
    }



    get selectedPage(): Page {
        if (!this.pages2edit || this.pages2edit.length == 0) {
            return undefined;
        }
        for (let i: number = 0; i < this.pages2edit.length; i++){
            if (this.pages2edit[i].selected){
                return this.pages2edit[i];
            }
        }
        return undefined;
    }

    get leftmostPageSelected(): boolean {
        let page: Page = this.selectedPage;
        if (!page){
            return false;
        }
        let result: boolean = this.pages2edit.indexOf(page) == 0;
        return result;
    }

    get rightmostPageSelected(): boolean {
        let page: Page = this.selectedPage;
        if (!page){
            return false;
        }
        let result: boolean = this.pages2edit.indexOf(page) == this.pages2edit.length - 1;
        return result;
    }


    shiftSelectedPageRight(): boolean {
       let page: Page = this.selectedPage;

        if (!page){
            return false;
        }

        if (this.rightmostPageSelected){
            return false;
        }

        let index: number = this.pages2edit.indexOf(page);
        this.pages2edit.splice(index,1);
        this.pages2edit.splice(index + 1, 0, page);
    }

    shiftSelectedPageLeft(): boolean {
    
       let page: Page = this.selectedPage;

        if (!page){
            return false;
        }

        if (this.leftmostPageSelected){
            return false;
        }

        let index: number = this.pages2edit.indexOf(page);
        this.pages2edit.splice(index,1);
        this.pages2edit.splice(index - 1, 0, page);
    }

    deletePage(page: Page): void {
        if (!this.pages2edit || !page) {
            return;
        }

        let index: number = this.pages2edit.indexOf(page);

        if (index < 0) {
            return;
        }

        let page2select: Page;
        let length = this.pages2edit.length;

        if (length > 1) {
            page2select = index > 0 ? this.pages2edit[index - 1] : this.pages2edit[1];
        }

        this.pages2edit.splice(index, 1);

        this.setPageSelected(page2select);
    }

    setPageSelected(page: Page) {

        if (!this.pages2edit || !page) {
            return;
        }

        let index: number = this.pages2edit.indexOf(page);

        if (index < 0) {
            return;
        }

        this.pages2edit.forEach((x) => {
            x.selected = x == page;
        });
    }

    addNewPage(): void {
        if (!this.pages2edit || this.pages2edit.length >= 500) {
            return undefined;
        }
        let page: Page = new Page();
        page.id = undefined;
        // TODO: translate New Page
        page.title = "New Page";
        this.pages2edit.push(page);
        this.setPageSelected(page);
    }

}