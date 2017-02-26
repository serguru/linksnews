import { Component, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DataService } from '../../core/services/data.service';
import { ActivatedRoute, UrlSegment } from '@angular/router';
import { Page } from "../../core/domain/page/page";
import { MessengerService } from "../../core/services/messenger.service";
import { Router } from '@angular/router';
import { AccountService } from "../../core/services/account.service";
import { Account } from "../../core/domain/account";
import { MyPagesEditorComponent } from './editor/my-pages-editor.component';
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription } from 'rxjs/Subscription';
import { PagesListComponent } from './pages-list.component';
import { PageCreateComponent } from './editor/page-create.component';
//import * as _ from 'lodash';  

import { ViewModes } from '../../core/common/enums';
import { UtilityService } from "../../core/services/utility.service";

@Component({
    
    templateUrl: './my-pages.component.html'
})

export class MyPagesComponent implements OnInit, OnDestroy {

    @ViewChild('pagesListComponent') pagesListComponent: PagesListComponent;

    @ViewChild('myPagesEditor') myPagesEditor: MyPagesEditorComponent;
    @ViewChild('pageCreate') pageCreate: PageCreateComponent;

   // subscription: Subscription;

    public _viewMode: ViewModes;

    get viewMode(): ViewModes {
        return this._viewMode;
    };

    set viewMode(value: ViewModes) {
        if (this._viewMode === value || !this.utilityService.viewModeValid(value)) {
            return;
        }
    
        this._viewMode = value;
        this.dataService.setCookie("myPagesViewMode", String(value), 
            ()=>{},
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }
            });
    }

    toString(): string {
        return 'MyPagesComponent';
    }

    get title(): string {
        return this.account ? this.account.login + "'s pages" : "customer's pages";
    }

    constructor (
                public dataService: DataService,
                public route: ActivatedRoute,
                public messengerService: MessengerService,
                public accountService: AccountService, 
                public router: Router,
                public appCommunicateService: AppCommunicateService,
                public utilityService: UtilityService
        ) {
        let viewMode = this.utilityService.getCookie("myPagesViewMode");

        if (this.utilityService.viewModeValid(viewMode)) {
            this.viewMode = +viewMode;
        } else {
            this.viewMode = ViewModes.List;
        }
    }

    pages: Array<Page>;

    public _editMode: boolean = false;

    get editMode(): boolean {
        return this._editMode;
    }

    set editMode(value: boolean) {
        if (this._editMode === value) {
            return;
        }

        if (!value) {
            this._editMode = false;
            return;
        }

        if (!this.accountService.isAccountAuthenticated) {
            return;
        }

        this._editMode = true;

        this.openEditor();
    }

    get account(): Account {
        return this.accountService.account;
    }

    public fillPages(data) {
        if (!data) {
            this.pages = undefined;
            return;
        }
        
        let pages: Array<Page> = new Array<Page>();

        for(let i: number = 0; i < data.length; i++) {
            pages.push(Page.from(data[i]));
        }

        this.pages = pages;
    }


    ngOnInit(){

        this.editMode = false;

        this.dataService.getMyPages(
            (data) => {
                this.fillPages(data);
                this.appCommunicateService.processCame(this);
            },
            (error) => {
                if (error && error.code === 401) {
                    this.messengerService.showOK("Login required",
                        "Please log in to have accsess to your own pages", () => {
                            this.router.navigate(["/login"])
                        });
                }
            });
    }

    ngOnDestroy(){
        this.appCommunicateService.processGone(this);
    }

    openEditor() {
        this.myPagesEditor.openEditor(this);
    }

    openCreate() {
        this.pageCreate.openEditor();
    }
}
