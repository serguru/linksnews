import { Component, OnInit, ViewChild, OnDestroy, AfterViewChecked, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import 'rxjs/add/operator/map';
import { AccountService } from './core/services/account.service';
import { Account } from './core/domain/account';
import { TranslateService, LangChangeEvent } from 'ng2-translate/ng2-translate';
import { DataService } from './core/services/data.service';
import { MessengerService } from './core/services/messenger.service';
import { UtilityService } from './core/services/utility.service';
import { MessageConfig } from  './core/domain/message-config';
import { DialogButtonType } from './core/common/enums';
import { DialogButtonConfig } from './core/domain/dialog-button-config';
import { MessengerComponent } from './components/messages/messenger.component';
import { WaitComponent } from './components/messages/wait.component';
import { Router, RoutesRecognized, NavigationEnd, NavigationStart,  NavigationExtras } from '@angular/router';
import { Language } from "./core/domain/language";
import { AppCommunicateService } from './core/services/app-communicate.service';
import { ViewModes } from './core/common/enums';

@Component({
    
    selector: 'links-news-app',
    templateUrl: './app.component.html'
})

export class AppComponent implements OnInit, OnDestroy, AfterViewChecked {


    pageTitlePrefix: string = "Links And News";

    viewModeComponents: Array<string> = [
        "MyPagesComponent",
        "CategoriesComponent",
        "NewsComponent"
        ];

    wait: boolean = false;

    $pageTitle: any;

    get pageTitle(): string {
        if (this.$pageTitle && this.$pageTitle.length > 0) {
            return this.$pageTitle.html();
        }
        return "";
    }

    set pageTitle(value: string) {
        if (!this.$pageTitle || this.$pageTitle.length === 0) {
            return;
        }
    
        if (!value) {
            this.$pageTitle.html(this.pageTitlePrefix);
            return;
        }

        this.translate.get(value).subscribe((translated) => {
            this.$pageTitle.html(this.pageTitlePrefix + ": " + translated);
        })
    }

    cancelTestingMode(event) {
        this.testingMode = false;
    }

    get navbarHeight(): number {
        if (!this.navbarVisible) {
            return 0;
        }
        return this.linksNavbar ? this.linksNavbar.outerHeight() : 0;
    }

    get currentLanguageCode(): string {
        return this.translate.currentLang;
    }
    
    zIndex: number = 2000;
    contentHeight: number;
    testingMode: boolean = false;

    public _navbarVisible: boolean;
    get navbarVisible(): boolean {
        return this._navbarVisible;
    };

    set navbarVisible(value: boolean) {
        if (this._navbarVisible === value) {
            return;
        }
    
        this._navbarVisible = value;
        this.dataService.setCookie("navbarVisible", String(value), 
            ()=>{},
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }
            }
            );
    }
    
    navbarTogglerLeft: number;

    activeComponent: any;

    @ViewChild("messengerComponent")
        messengerComponent: MessengerComponent;

    @ViewChild("waitComponent")
        waitComponent: WaitComponent;

    @ViewChild('navbarToggler')
        navbarToggler;

    linksNavbar: any;

    setControlsProps() {
        setTimeout(() => {

            let viewportHeight: number = window.innerHeight;
            let contentHeight: number = viewportHeight - this.navbarHeight;

            if (contentHeight !== this.contentHeight) {
                this.contentHeight = viewportHeight - this.navbarHeight;
            }

            let togglerWidth: number = this.navbarToggler && this.navbarToggler.nativeElement.offsetWidth > 0 ?
                this.navbarToggler.nativeElement.offsetWidth : 29;

            let navbarTogglerLeft: number = window.innerWidth - togglerWidth - 20;

            if (navbarTogglerLeft !== this.navbarTogglerLeft) {
                this.navbarTogglerLeft = navbarTogglerLeft;
            }
        }, 0);
    }

    ngOnInit() {
        this.$pageTitle = jQuery("html").find('title');
        this.pageTitle = "";
        this.linksNavbar = jQuery("#linksNavbar");
        this.setControlsProps();
        this.messengerService.messenger = this.messengerComponent;
        this.messengerService.wait = this.waitComponent;
    }

    get account(): Account {
        return this.accountService.account;
    }

    setCommunications() {
        this.appCommunicateService.came$.subscribe((component) => {
            this.activeComponent = component; 
            if (!component) {
                return;
            }

            this.pageTitle = this.activeComponent.title;
        });

        this.appCommunicateService.gone$.subscribe((component) => {
            this.activeComponent = undefined; 
            this.pageTitle = "";
        });

        this.appCommunicateService.wait$.subscribe((value) => {
            this.wait = value;
        });

    }

    constructor(
        public accountService: AccountService, 
        public location: Location,
        public translate: TranslateService, 
        public messengerService: MessengerService, 
        public dataService: DataService, 
        public router: Router,
        public utilityService: UtilityService,
        public appCommunicateService: AppCommunicateService
        ) { 
            this.setCommunications();

            this.translate.setDefaultLang('en');
            this.translate.onLangChange.subscribe((e: LangChangeEvent) => {
                this.dataService.setLanguage(e.lang,(error) => {
                    if (error && error.message) {
                        this.messengerService.showError("Error", error.message);
                    }
                });
            });

            this.accountService.getInterfaceLanguages();
            this.accountService.refreshAccount(() => {
                this.accountService.registerVisit("came");
            });

            router.events.subscribe((event) => {
                if (event instanceof NavigationStart) {
                    this.wait = false;
                    this.accountService.registerVisit(event.url);
                }
            });
            window.onbeforeunload = () => { 
                this.onWindowUnload();
            };

            let navbarVisible: string = this.utilityService.getCookie("navbarVisible");
            this.navbarVisible = !navbarVisible || navbarVisible === "true" ? true : false;
    }

    ngOnDestroy(){
    }

    activeComponentOf(type: string): any {
        if (!this.activeComponent) {
            return undefined;
        }

        if (this.activeComponent.toString() === type) {
            return this.activeComponent;
        }

        return undefined;
    }

    get editablePageShown(): boolean {
        return this.activeComponentOf("PageComponent") ? this.accountService.isAccountAuthenticated : false;
    }

    get editPageAllowed(): boolean {

        return this.editablePageShown && 
            this.accountService.account.login === this.activeComponent.page.login;
    }

    get add2MyPagesAllowed(): boolean {
        return this.editablePageShown && 
            this.accountService.account.login !== this.activeComponent.page.login;
    }

    get editPageListAllowed(): boolean {
        return this.activeComponentOf("MyPagesComponent") && this.accountService.isAccountAuthenticated ?
            true : false;
    }

    add2MyPages(): void {
        this.dataService.add2MyPages(this.activeComponent.page.id, 
            () => {
                this.messengerService.showOK("Add page", "The page has been added to your pages");
            },
            (error) => {
                if (error && error.message) {
                    this.messengerService.showError("Error", error.message);
                }
            }
            );
    }

    copy2MyPages(): void {

        this.messengerService.showOkCancel("Copying page","Do you want to copy a page?",
            () => {
                this.messengerService.showWait("Copying a page...", () => {
                    this.dataService.copy2MyPages(this.activeComponent.page.id, 
                        () => {
                            this.messengerService.closeWait(() => {
                                this.messengerService.showOK("Copying page", "The page has been copied to your pages");
                            });
                        },
                        (error) => {

                            if (error && error.message) {
                                this.messengerService.closeWait(() => {
                                    this.messengerService.showError("Error", error.message);
                                });
                                return;
                            }

                            this.messengerService.closeWait();
                        }
                    );
                });
            });
    }

    toggleEditMode() {
        if (!this.editPageAllowed && !this.editPageListAllowed) {
            return;
        }

        this.activeComponent.editMode = !this.activeComponent.editMode;
    }

    createPage() {
        this.activeComponent.openCreate();
    }


    logUserOut(): void {
        this.accountService.logout(() => {
           this.accountService.setCurrentLanguage();
           this.router.navigate(["home"]);
        });
    }

    toggleNavbar(value: boolean){
        this.navbarVisible = value;
        
        setTimeout(() => {
                this.setControlsProps();
            }, 100
        );
    }

    toEllipsis(value: string): string {
        if (!value) {
            return value;
        }

        if (value.length <= 23) {
            return value;
        }
        
        return value.substr(0, 20) + "...";
    }

    oldNavbarHeight: number;

    ngAfterViewChecked() {
        if (this.oldNavbarHeight !== this.navbarHeight) {
            this.oldNavbarHeight = this.navbarHeight;
            this.setControlsProps();
        }
    }


    onWindowResize() {
        this.setControlsProps();
    }

    onWindowUnload() {
        // This did not work here
        // this.accountService.registerVisit("gone");

        //xhr.setRequestHeader("enctype", "multipart/form-data");
        //// IE workaround for Cache issues
        //xhr.setRequestHeader("Cache-Control", "no-cache");
        //xhr.setRequestHeader("Cache-Control", "no-store");
        //xhr.setRequestHeader("Pragma", "no-cache");

        let accountId: number = this.account ? this.account.id : undefined;
        let xhr = new XMLHttpRequest()
        xhr.open("POST","home/registerVisit",false); // did not work with true?
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.setRequestHeader('Accept', 'application/json');
        let data = JSON.stringify({
            route: "gone", 
            accountId: accountId
        });
        xhr.send(data);
    }

    setViewMode(viewMode: ViewModes) {
        if (!this.activeComponent || this.viewModeComponents.indexOf(this.activeComponent.toString()) < 0) {
            return; 
        }
        return this.activeComponent.viewMode = viewMode;
    }

    getViewMode(): ViewModes {
        if (!this.activeComponent || this.viewModeComponents.indexOf(this.activeComponent.toString()) < 0) {
            return undefined;
        }
        return this.activeComponent.viewMode;
    }
}
