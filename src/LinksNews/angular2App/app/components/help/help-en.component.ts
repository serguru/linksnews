import { Component, OnInit, OnDestroy, ViewChild, Input, ViewChildren, AfterViewChecked, QueryList, ElementRef } from '@angular/core';
import { Page } from "../../core/domain/page/page";
import { Router, ActivatedRoute } from '@angular/router';
import { DataService } from '../../core/services/data.service';
import { AccountService } from '../../core/services/account.service';
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription }   from 'rxjs/Subscription';
import { TranslateService, LangChangeEvent } from 'ng2-translate/ng2-translate';
import { HelpComponent } from "./help.component"

//import * as _ from 'lodash';  

@Component({
    selector: "help-en-placeholder",
    
    templateUrl: './help-en.component.html',
})

export class HelpEnComponent implements OnInit, OnDestroy {

    toString(): string {
        return 'HelpEnComponent';
    }

    //@ViewChildren('section') _sections: QueryList<ElementRef>;
    @Input() component: HelpComponent;

    constructor(
        public route: ActivatedRoute, 
        public dataService: DataService,
        public router: Router,
        public accountService: AccountService,
        public appCommunicateService: AppCommunicateService
        ) { 
    }

    ngOnInit(){
        this.component.setSections();
    }

    ngOnDestroy(){
        this.component.sections = undefined;
    }
}