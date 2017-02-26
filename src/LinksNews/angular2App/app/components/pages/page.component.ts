import { Component, OnInit, OnDestroy, ViewChild, ViewChildren, AfterViewChecked, QueryList } from '@angular/core';
import { PageEditorComponent } from './editor/page-editor.component';
import { Page } from "../../core/domain/page/page";
import { Router, ActivatedRoute } from '@angular/router';
import { DataService } from '../../core/services/data.service';
import { AccountService } from '../../core/services/account.service';
import { ColumnComponent } from './column.component';
import { ColumnTypes } from "../../core/common/enums";
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription } from 'rxjs/Subscription';
import { Account } from "../../core/domain/account";
import { ColumnEditorComponent } from "./editor/column-editor.component";
import { RowEditorComponent } from "./editor/row-editor.component";
import { LinkEditorComponent } from "./editor/link-editor.component";
import { MessengerService } from "../../core/services/messenger.service";

import * as _ from 'lodash';

@Component({
    
    templateUrl: './page.component.html',
})

export class PageComponent implements OnInit, OnDestroy {
    subscription: Subscription;

    @ViewChild("linkEditor")
    linkEditor: LinkEditorComponent;

    @ViewChild("columnEditor")
    columnEditor: ColumnEditorComponent;

    @ViewChild("rowEditor")
    rowEditor: RowEditorComponent;

    toString(): string {
        return 'PageComponent';
    }

    get title(): string {
        return this.page ? this.page.title : "page";
    }

    constructor(
        public route: ActivatedRoute,
        public dataService: DataService,
        public router: Router,
        public accountService: AccountService,
        public appCommunicateService: AppCommunicateService,
        public messengerService: MessengerService
    ) {
        this.subscription = this.appCommunicateService.came$.subscribe(
            (component) => {
                // do something
            }
        )

        this.subscription = this.appCommunicateService.gone$.subscribe(
            (component) => {
                // do something
            }
        )
    }

    @ViewChildren(ColumnComponent) columnComponents: QueryList<ColumnComponent>;

    @ViewChild("adPlaceholder") adPlaceholder;

    _editMode: boolean = false;

    get editMode(): boolean {
        return this._editMode;
    }

    set editMode(value: boolean) {
        if (this._editMode === value) {
            return;
        }

        let account: Account = this.accountService.account;

        if (!account) {
            this._editMode = false;
            return;
        }

        let login: string = account.login;

        this._editMode = value;

        if (!value) {
            this.getPage(true);
        }
    }

    public _page: Page;
    get page(): Page {
        return this._page;
    }

    set page(page: Page) {
        if (this._page != page) {
            this._page = page;
        }
    }

    sub: any;
    loginName: string;
    pageName: string;

    refreshNews() {
        if (!this.columnComponents) {
            return;
        }

        this.dataService.removePageFromCache(this.loginName, this.pageName,
            () => {
                this.columnComponents.forEach((x) => {
                    x.refreshNews();
                });
            },
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }
            }
        );
    }

    //interval: NodeJS.Timer;
    interval: any;

    getPage(refreshCache: boolean) {
        this.appCommunicateService.processWait(true);
        this.dataService.getPage(this.loginName, this.pageName, refreshCache,
            (data) => {
                this.appCommunicateService.processWait(false);

                if (_.isEmpty(data)) {
                    this.router.navigate(['404']);
                    return;
                }

                let page: Page = Page.from(<Page>data);
                this.page = page;

                this.appCommunicateService.processCame(this);

                if (this.accountService.account && this.accountService.account.newsRefreshInterval >= 10) {
                    this.interval = setInterval(() => {
                        this.refreshNews();
                    }, this.accountService.account.newsRefreshInterval * 60 * 1000);
                }
            },

            (error) => {
                this.appCommunicateService.processWait(false);
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }
            }
        );
    }

    ngOnInit() {
        this.sub = this.route.params.subscribe(params => {
            this.loginName = params['login'];
            this.pageName = params['page'];
            this.getPage(false);
        });
    }

    ngOnDestroy() {
        if (this.interval) {
            clearInterval(this.interval);
        }
        this.sub.unsubscribe();
        this.appCommunicateService.processGone(this);
        this.subscription.unsubscribe();
    }

    @ViewChild('pageEditor')
    editor: PageEditorComponent;

    openPageEditor() {
        this.editor.openPageEditor(this);
    }
}