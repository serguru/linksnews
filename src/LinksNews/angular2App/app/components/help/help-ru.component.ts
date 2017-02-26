import { Component, OnInit, OnDestroy, ViewChild, Input, ViewChildren, AfterViewChecked, QueryList, ElementRef } from '@angular/core';
import { Page } from "../../core/domain/page/page";
import { Router, ActivatedRoute } from '@angular/router';
import { DataService } from '../../core/services/data.service';
import { AccountService } from '../../core/services/account.service';
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { TranslateService, LangChangeEvent } from 'ng2-translate/ng2-translate';
import { Subscription }   from 'rxjs/Subscription';
import { HelpComponent } from "./help.component"
//import * as _ from 'lodash';  

@Component({
    selector: "help-ru-placeholder",
    
    templateUrl: './help-ru.component.html',
})

export class HelpRuComponent implements OnInit, OnDestroy  {

//    @ViewChildren('section') _sections: QueryList<ElementRef>;

    toString(): string {
        return 'HelpRuComponent';
    }

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