import { Component, OnInit, OnDestroy, ViewChild, Input, ViewChildren, AfterViewChecked, QueryList, ElementRef } from '@angular/core';
import { Page } from "../../core/domain/page/page";
import { Router, ActivatedRoute } from '@angular/router';
import { DataService } from '../../core/services/data.service';
import { AccountService } from '../../core/services/account.service';
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription }   from 'rxjs/Subscription';
import { TranslateService, LangChangeEvent } from 'ng2-translate/ng2-translate';
import { SiteNewsComponent } from "./site-news.component"

//import * as _ from 'lodash';  

@Component({
    selector: "site-news-ru-placeholder",
    
    templateUrl: './site-news-ru.component.html',
})

export class SiteNewsRuComponent implements OnInit, OnDestroy {

    toString(): string {
        return 'SiteNewsRuComponent';
    }

    @Input() component: SiteNewsComponent;

    constructor(
        public route: ActivatedRoute, 
        public dataService: DataService,
        public router: Router,
        public accountService: AccountService,
        public appCommunicateService: AppCommunicateService
        ) { 
    }


    ngOnInit(){
    }

    ngOnDestroy(){
    }
}