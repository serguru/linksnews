import { Component, OnInit, OnDestroy, ViewChild, ViewChildren, AfterViewChecked, AfterViewInit, AfterContentInit, QueryList, ElementRef } from '@angular/core';
import { Page } from "../../core/domain/page/page";
import { Router, ActivatedRoute } from '@angular/router';
import { DataService } from '../../core/services/data.service';
import { AccountService } from '../../core/services/account.service';
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription }   from 'rxjs/Subscription';
import { TranslateService, LangChangeEvent } from 'ng2-translate/ng2-translate';

//import * as _ from 'lodash';  

@Component({
    
    templateUrl: './site-news.component.html'
})

export class SiteNewsComponent implements OnInit, OnDestroy, AfterViewChecked, AfterViewInit, AfterContentInit  {

    subscription: Subscription;

    contentHeight: number;

    toString(): string {
        return 'SiteNewsComponent';
    }

    get title(): string {
        return "site news";
    }


    constructor(
        public route: ActivatedRoute, 
        public dataService: DataService,
        public router: Router,
        public accountService: AccountService,
        public translate: TranslateService,
        public appCommunicateService: AppCommunicateService
        ) { 

        this.subscription = this.appCommunicateService.contentHeightChanged$.subscribe (
            (height) => {
                this.contentHeight = height;
            }
        )
    }

    ngAfterViewInit() {
    }

    ngAfterContentInit() {
    }

    ngAfterViewChecked() {
    }

    ngOnInit(){
        this.appCommunicateService.processCame(this);
    }

    ngOnDestroy(){
        this.appCommunicateService.processGone(this);
        this.subscription.unsubscribe();
    }

}