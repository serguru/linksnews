import { Component, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DataService } from '../../core/services/data.service';
import { ActivatedRoute, UrlSegment } from '@angular/router';
import { Page } from "../../core/domain/page/page";
import { MessengerService } from "../../core/services/messenger.service";
import { Router } from '@angular/router';
import { AccountService } from "../../core/services/account.service";
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription }   from 'rxjs/Subscription';
import { NewsSource } from '../../core/domain/news-source';
import { Link } from "../../core/domain/page/link";
//import * as _ from 'lodash';  
import { ViewModes, SortOrder } from '../../core/common/enums';
import { UtilityService } from "../../core/services/utility.service";
import { TranslateService, LangChangeEvent } from 'ng2-translate/ng2-translate';
import { SortBy } from "../../core/domain/sort-by";

@Component({
    
    templateUrl: './news.component.html'
})

export class NewsComponent implements OnInit, OnDestroy {

    subParams: Subscription;

    newsSourcesWithHeader: Array<NewsSource>;
    sourceId: string;
    links: Array<Link>;



    public _viewMode: ViewModes;

    get viewMode(): ViewModes {
        return this._viewMode;
    };

    set viewMode(value: ViewModes) {
        if (this._viewMode === value || !this.utilityService.viewModeValid(value)) {
            return;
        }
    
        this._viewMode = value;
        this.dataService.setCookie("newsViewMode", String(value), 
            ()=>{},
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }
            });
    }

    constructor (
                public dataService: DataService,
                public route: ActivatedRoute,
                public messengerService: MessengerService,
                public accountService: AccountService, 
                public router: Router,
                public appCommunicateService: AppCommunicateService,
                public utilityService: UtilityService,
                public translate: TranslateService 
        ) {
        let viewMode = this.utilityService.getCookie("newsViewMode");

        if (this.utilityService.viewModeValid(viewMode)) {
            this.viewMode = +viewMode;
        } else {
            this.viewMode = ViewModes.List;
        }
    }

    toString(): string {
        return 'NewsComponent';
    }

    get title(): string {
        return "fresh news";
    }

    get selectSourcesVisible(): boolean {
        return window.innerWidth < 768;
    }

    onSelectSource(event) {
        setTimeout(() => {
            //let sourceId: string = this.sourceId || "";
            //sourceId = sourceId.toLowerCase() === "news sources" ? "" : sourceId;
            this.router.navigateByUrl('/news/' + this.sourceId);
        },0);
    }

    findSourceBySourceId(sourceId: string): NewsSource {
        if (!sourceId) {
            return undefined;
        }
        sourceId = sourceId.toLowerCase();
        if (this.newsSourcesWithHeader) {
            for (let i: number = 0; i < this.newsSourcesWithHeader.length; i++) {
                if (this.newsSourcesWithHeader[i].newsSourceId.toLowerCase() === sourceId) {
                    return this.newsSourcesWithHeader[i];
                }
            }
        }
        return undefined;
    }


    processParams(params) {
        let sourceId = params['sourceId'];

        if (!sourceId) {
            this.sourceId = undefined;
            return;
        }

        let source: NewsSource = this.findSourceBySourceId(sourceId);
        if (!source) {
            this.router.navigate(['404']);
            return;
        }

        this.sourceId = source.newsSourceId;

        if (sourceId.toLowerCase() === "sources") {
            return;
        }

        if (source.links) {
            this.links = source.links;
        
        }

        //this.appCommunicateService.processWait(true);
        this.dataService.getLinksBySource(source, 
            (data) => {
                //  this.appCommunicateService.processWait(false);
                if (!data) {
                    this.router.navigate(['404']);
                    return;
                }
        
                let links: Array<Link> = new Array<Link>();

                for(let i: number = 0; i < data.length; i++) {
                    links.push(Link.from(data[i]));
                }

                source.links = links;
                this.links = links;
            },
            (error) => {
                //  this.appCommunicateService.processWait(false);
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }                
            }
        );
    }

    ngOnInit(){

        this.appCommunicateService.processWait(true);
        this.dataService.getNewsSources(
            (data) => {
                
                this.newsSourcesWithHeader = new Array<NewsSource>();
                
                let allSources: NewsSource = new NewsSource();
                allSources.id = -1;
                allSources.newsSourceId = "sources";
                this.newsSourcesWithHeader.push(allSources);

                data.forEach((x) => {
                    this.newsSourcesWithHeader.push(NewsSource.from(x));
                });                    

                this.subParams = this.route.params.subscribe((params) => {
                    this.processParams(params);
                });
                this.appCommunicateService.processWait(false);
                this.appCommunicateService.processCame(this);
            },
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }                
            }
        );
    }

    ngOnDestroy(){
        this.appCommunicateService.processGone(this);
        if (this.subParams) {
            this.subParams.unsubscribe();
        }
    }
}
