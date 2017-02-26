import { Component, Input, ViewChild, OnChanges, SimpleChanges } from '@angular/core';
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
import { AccountService } from "../../../core/services/account.service";
import { Account } from "../../../core/domain/account";
import { validationPattern } from "../../../core/common/constant";
import { FileUploadComponent } from "./file-upload.component";
import { ContentCategory } from "../../../core/domain/content-category";
import { MyPagesComponent } from "../my-pages.component";

@Component({
    selector: 'page-create-placeholder',
    
    templateUrl: './page-create.component.html',
})

export class PageCreateComponent {

    data: any;

    zIndex: number;

    constructor(
        public messengerService: MessengerService, 
        public dataService: DataService,
        public router: Router,
        public accountService: AccountService
        ) { 
        this.data = {
            pageTitle: undefined,
            linkTitle: undefined,
            linkAddress: undefined
        }
    }

    @ViewChild('modalCreatePageEditor')
        editor: any;

    onEditorOpen() {
        this.messengerService.zIndex = this.messengerService.zIndex + 2;
        this.zIndex = this.messengerService.zIndex;
        jQuery("div.modal-backdrop").last().css("z-index", this.zIndex - 1);

    }

    openEditor() {
        this.data = {
            pageTitle: undefined,
            linkTitle: undefined,
            linkAddress: undefined
        }
        this.editor.open();
    }

    close() {
        this.editor.dismiss();
    }

    save() {

        let account: Account = this.accountService.account;

        if (!account || !account.login) {
            this.messengerService.showWarning("Page create", "You should be logged in to create a page");
            return;
        }

        this.dataService.createPage(this.data, 
                    (pageName) => {
                        this.editor.dismiss().then(() => {
                                this.messengerService.showOK("Page create", "Page has been created", () => {
                                    this.router.navigate(["/page", account.login, pageName]);
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