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
import { ColumnEditorComponent } from "../pages/editor/column-editor.component";

@Component({
    selector: 'news-sources-placeholder',
    
    templateUrl: './news-sources.component.html'
})

export class NewsSourcesComponent implements OnInit, OnDestroy {

    @Input("selectable") selectable: boolean;
    @Input("showWait") showWait: boolean;
    @Input("columnEditor") columnEditor: ColumnEditorComponent;

    subLang: Subscription;
    newsSources: Array<NewsSource>;

    sortFields: Array<string> = [
        "newsSourceId",
        "contentCategoryTranslated",
        "languageNameTranslated",
        "countryNameTranslated"
    ]

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

        this.subLang = this.translate.onLangChange.subscribe((e: LangChangeEvent) => {
                this.translateSources(() => {
                    this.refreshSortOrder();
                });    
        });
    }

    toString(): string {
        return 'NewsComponent';
    }


    _selectedSource: NewsSource;

    get selectedSource(): NewsSource {
        return this._selectedSource;
    }

    set selectedSource(value: NewsSource) {
        if (this._selectedSource != value) {
            this._selectedSource = value;
        }
    }

    setSelectedSourceById(sourceId: string) {

        if (!this.newsSources || !sourceId) {
            this.selectedSource = undefined;
            return;
        }

        let source: NewsSource;

        for (let i: number = 0; i < this.newsSources.length; i++) {
            if (this.newsSources[i].newsSourceId === sourceId) {
                source = this.newsSources[i];
                break;
            }
        }

        this.selectedSource = source;
    }

    translateList(keys: Array<string>, callabck: Function) {
        this.translate.get(keys).subscribe((translated) => {
            callabck(translated);
        });    
    }

    sortNewsSources(name: string) {
        this.sortBy = new SortBy(name, this.sortBy.name === name ? this.sortBy.next() : SortOrder.Asc);
    }

    getSortHidden(name: string, sortOrder?: SortOrder): boolean {
        if (!this.sortBy) {
            return false;
        }

        if (!sortOrder) {
            let result: boolean = this.sortBy.name === name && (this.sortBy.sortOrder !== SortOrder.None);
            return result;
        }

        let result: boolean = this.sortBy.name !== name || this.sortBy.sortOrder != sortOrder;
        return result;
    }


    refreshSortOrder(value?: SortBy) {

        if (!this.newsSources || this.newsSources.length === 0) {
            return;
        }

        if (!value) {
            value = this.sortBy;
        }

        if (value.sortOrder === SortOrder.Asc) {
            this.newsSources.sort((a, b) => {
                if (a[value.name] < b[value.name]) {
                    return -1;
                }
                if (a[value.name] > b[value.name]) {
                    return 1;
                }
                if (a["newsSourceId"] < b["newsSourceId"]) {
                    return -1;
                }
                if (a["newsSourceId"] > b["newsSourceId"]) {
                    return 1;
                }
                return 0;
            })       
        } else if (value.sortOrder === SortOrder.Desc) {
            this.newsSources.sort((a, b) => {
                if (a[value.name] > b[value.name]) {
                    return -1;
                }
                if (a[value.name] < b[value.name]) {
                    return 1;
                }
                if (a["newsSourceId"] < b["newsSourceId"]) {
                    return -1;
                }
                if (a["newsSourceId"] > b["newsSourceId"]) {
                    return 1;
                }
                return 0;
            })       
        } else {
            this.newsSources.sort((a, b) => {
                if (a["newsSourceId"] < b["newsSourceId"]) {
                    return -1;
                }
                if (a["newsSourceId"] > b["newsSourceId"]) {
                    return 1;
                }
                return 0;
            })       
        }
    }

    public _sortBy: SortBy = new SortBy(undefined, SortOrder.None);

    get sortBy(): SortBy {
        return this._sortBy;
    }

    set sortBy(value: SortBy) {

        if (this._sortBy.name === value.name && this._sortBy.sortOrder === value.sortOrder) {
            return;
        }
        this.refreshSortOrder(value);

        let cookie: string = JSON.stringify(value);
        this.dataService.setCookie("newsSourcesSortOrder", cookie, 
            ()=>{},
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }
            });
        this._sortBy = value;
    }

    translateSources(callback?: Function) {
        
        if (!this.newsSources) {
            return;
        }
        
        let keys: Array<string> = new Array<string>();

        this.newsSources.forEach((x) => {
            if (x.contentCategory && keys.indexOf(x.contentCategory) < 0) {
                keys.push(x.contentCategory);
            }
            if (x.countryName && keys.indexOf(x.countryName) < 0) {
                keys.push(x.countryName);
            }
            if (x.languageName && keys.indexOf(x.languageName) < 0) {
                keys.push(x.languageName);
            }
        })

        this.translateList(keys, (x) => {

            this.newsSources.forEach((y) => {
                y.contentCategoryTranslated = x[y.contentCategory];
                y.countryNameTranslated = x[y.countryName];
                y.languageNameTranslated = x[y.languageName];
            })

            if (callback) {
                callback();
            }
        });
    }

    sortByValid(sortBy: SortBy): boolean {
        if (!sortBy) {
            return false;
        }

        if (this.sortFields.indexOf(sortBy.name) < 0) {
            return false;
        }

        if ([0,1,2].indexOf(sortBy.sortOrder) < 0) {
            return false;
        }
        return true;
    }


    public processError(error) {
        if (error && error.message) {
            this.messengerService.showError("Error", error.message);
        }                
    }

    public doTranslate() {
        this.translateSources(() => {

            let sortBy: SortBy;

            let cookie: string = this.utilityService.getCookie("newsSourcesSortOrder");
            if (cookie) {
                try {
                    sortBy = JSON.parse(cookie);
                } catch(e) {
                    sortBy = undefined;
                }
            }

            if (this.sortByValid(sortBy)) {
                this.sortBy = new SortBy(sortBy.name, sortBy.sortOrder);
            } else {
                this.sortBy = new SortBy("newsSourceId", SortOrder.None);
            }
        });    
    }

    public doInit() {
        this.dataService.getNewsSources(
            (data) => {
                this.newsSources = new Array<NewsSource>();

                data.forEach((x) => {
                    this.newsSources.push(NewsSource.from(x));
                });                    

                if (this.columnEditor) {
                    this.setSelectedSourceById(this.columnEditor.column2edit.newsProviderSourceId);
                }


                if (this.showWait) {
                    this.messengerService.closeWait(()=> {
                        this.doTranslate();
                    });
                    return;
                }
                this.doTranslate();
            },
            (error) => {
                if (this.showWait) {
                    this.messengerService.closeWait(()=> {
                        this.processError(error);
                    })
                    return;
                }
                this.processError(error);
            }
        );
    }

    ngOnInit(){

        if (this.showWait) {
            this.messengerService.showWait("Getting news sources...", () => {
                this.doInit();
            });
            return;
        }

        this.doInit();
    }

    ngOnDestroy(){
        if (this.subLang) {
            this.subLang.unsubscribe();
        }
    }

}
