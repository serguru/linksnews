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
    
    templateUrl: './help.component.html',
})

export class HelpComponent implements OnInit, OnDestroy, AfterViewChecked, AfterViewInit, AfterContentInit  {

    subFragment: Subscription;
    sections: Array<any>;
    fragment: string;

    toString(): string {
        return 'HelpComponent';
    }

    get title(): string {
        return "help";
    }


    constructor(
        public route: ActivatedRoute, 
        public dataService: DataService,
        public router: Router,
        public accountService: AccountService,
        public translate: TranslateService,
        public appCommunicateService: AppCommunicateService
        ) { 
    }

    ngAfterViewInit() {
    }

    ngAfterContentInit() {
    }

    ngAfterViewChecked() {
    }

    setSections() {
        
        setTimeout(() => {
        
            let sections = [];

            let headers = jQuery('.link-help-content h5');

            if (headers.length > 0) {
                headers.each((x, y) => {
                    if (y.id) {
                        sections.push({
                            id: y.id,
                            name: y.innerHTML
                        })
                    }
                });
                this.sections = sections;
                setTimeout(()=> {
                    this.goTo(this.fragment);
                },0);
                
            }
        },0);
    }

    goTo(id) {
        this.fragment = id;
        if (!id) {
            return;
        }

        let header = jQuery(".link-help-content #" + id);
        if (header.length === 0) {
            return;
        }
        jQuery("#helpContetnDiv").scrollTop(header[0].offsetTop);
    }

    onAnchorClick(event, id) {
        event.preventDefault();
        this.goTo(id);
    }

    ngOnInit(){
        this.appCommunicateService.processCame(this);
        this.subFragment = this.route.fragment.subscribe(fragment => {
            this.goTo(fragment);
        });
    }

    ngOnDestroy(){
        this.appCommunicateService.processGone(this);
        if (this.subFragment) {
            this.subFragment.unsubscribe();
        }
    }

}