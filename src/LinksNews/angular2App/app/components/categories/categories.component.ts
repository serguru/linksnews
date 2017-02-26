import { Component, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DataService } from '../../core/services/data.service';
import { ActivatedRoute, UrlSegment } from '@angular/router';
import { Page } from "../../core/domain/page/page";
import { MessengerService } from "../../core/services/messenger.service";
import { Router } from '@angular/router';
import { AccountService } from "../../core/services/account.service";
import { Account } from "../../core/domain/account";
import { AppCommunicateService } from '../../core/services/app-communicate.service';
import { Subscription }   from 'rxjs/Subscription';
import { ContentCategory } from '../../core/domain/content-category';
//import * as _ from 'lodash';  
import { ViewModes } from '../../core/common/enums';
import { UtilityService } from "../../core/services/utility.service";

@Component({
    
    templateUrl: './categories.component.html'
})

export class CategoriesComponent implements OnInit, OnDestroy {

    subParams: Subscription;
    categories: Array<ContentCategory>;
    categoryName: string;
    pages: Array<Page>;

    public _viewMode: ViewModes;

    get viewMode(): ViewModes {
        return this._viewMode;
    };

    get title(): string {
        return "categories";
    }



    set viewMode(value: ViewModes) {
        if (this._viewMode === value || !this.utilityService.viewModeValid(value)) {
            return;
        }
    
        this._viewMode = value;
        this.dataService.setCookie("categoriesViewMode", String(value), 
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
                public utilityService: UtilityService
        ) {

        let viewMode = this.utilityService.getCookie("categoriesViewMode");

        if (this.utilityService.viewModeValid(viewMode)) {
            this.viewMode = +viewMode;
        } else {
            this.viewMode = ViewModes.List;
        }
    }

    toString(): string {
        return 'CategoriesComponent';
    }

    get selectCategoriesVisible(): boolean {
        return window.innerWidth < 768;
    }

    get account(): Account {
        return this.accountService.account;
    }

    findCategoryByName(categoryName: string): ContentCategory {
        if (!categoryName) {
            return undefined;
        }
        categoryName = categoryName.toLowerCase();
        if (this.categories) {
            for (let i: number = 0; i < this.categories.length; i++) {
                if (this.categories[i].name.toLowerCase() === categoryName) {
                    return this.categories[i];
                }
            }
        }
        return undefined;
    }

    onSelectCategory(event) {
        setTimeout(() => {
            this.router.navigateByUrl('/categories/' + this.categoryName);
        },0);
    }

    processParams(params) {

        let categoryName: string = params['category'];

        if (!categoryName) {
            this.categoryName = undefined;
            return;
        }

        let category: ContentCategory = this.findCategoryByName(categoryName);
        if (!category) {
            this.router.navigate(['404']);
            return;
        }

        this.categoryName = category.name;

        if (categoryName.toLowerCase() === "all") {
            return;
        }
                    
        if (category.pages) {
            this.pages = category.pages;
            return;
        }

        this.appCommunicateService.processWait(true);
        this.dataService.getPagesByCategory(category.name, 
            (data) => {
                this.appCommunicateService.processWait(false);
                if (!data) {
                    this.router.navigate(['404']);
                    return;
                }
        
                let pages: Array<Page> = new Array<Page>();

                for (let i: number = 0; i < data.length; i++) {
                    pages.push(Page.from(data[i]));
                }

                category.pages = pages;
                this.pages = pages;
            },
            (error) => {
                this.appCommunicateService.processWait(false);
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }                
            }
        );
    }

    ngOnInit(){

        this.appCommunicateService.processWait(true);
        this.dataService.getContentCategories(
            (data) => {
                this.categories = data;
                this.subParams = this.route.params.subscribe((params) => {
                    this.processParams(params);
                });
                this.appCommunicateService.processWait(false);
                this.appCommunicateService.processCame(this);
            },
            (error) => {
                this.appCommunicateService.processWait(false);
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
