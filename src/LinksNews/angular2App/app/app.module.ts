import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule }    from '@angular/forms';
import { Location, LocationStrategy, HashLocationStrategy } from '@angular/common';
import { Headers, RequestOptions, BaseRequestOptions} from '@angular/http';
import { routing } from './app.routes';
import { AppComponent }  from './app.component';
import { HttpModule, JsonpModule, Http } from '@angular/http';
import { TranslateModule, TranslateStaticLoader, TranslateLoader } from 'ng2-translate';
import { HomeComponent } from './components/facade/home.component';
import { DataService } from './core/services/data.service';
import { UtilityService } from './core/services/utility.service';
import { UnderConstructionComponent } from './components/under-construction.component';
import { PageComponent } from './components/pages/page.component';
import { ColumnComponent } from './components/pages/column.component';
import { RowComponent } from './components/pages/row.component';
import { LinkComponent } from './components/pages/link.component';
import { PagesListComponent } from './components/pages/pages-list.component';
import { HideIfFalsePipe } from './core/common/pipes';
import { PageEditorComponent } from './components/pages/editor/page-editor.component';
import { MessengerService } from './core/services/messenger.service';
import { MessengerComponent } from './components/messages/messenger.component';
import { DialogButtonComponent } from "./components/messages/dialog-button.component";
import { ColumnEditorComponent } from './components/pages/editor/column-editor.component';
import { RowListEditorComponent } from './components/pages/editor/row-list-editor.component';
import { LinkListEditorComponent } from './components/pages/editor/link-list-editor.component';
import { LinkEditorComponent } from './components/pages/editor/link-editor.component';
import { RowEditorComponent } from './components/pages/editor/row-editor.component';
import { ColumnListEditorComponent } from './components/pages/editor/column-list-editor.component';
import { NewsSourceSelectorComponent } from './components/news/news-source-selector.component';
import { AccountService } from './core/services/account.service';
import { LoginComponent } from './components/account/login.component';
import { RegisterComponent }   from './components/account/register.component';
import { AccountComponent } from './components/account/account.component';
import { NotFoundComponent } from './components/not-found.component';
import { MyPagesEditorComponent } from './components/pages/editor/my-pages-editor.component';
import { ColumnPresenterComponent } from './components/pages/editor/column-presenter.component';
import { RowPresenterComponent } from './components/pages/editor/row-presenter.component';
import { LinkPresenterComponent } from './components/pages/editor/link-presenter.component';
import { SpaceValidatorDirective } from './core/directives/space-validation.directive';
import { UrlStartValidatorDirective } from './core/directives/url-start-validation.directive';
import { FileUploadComponent } from './components/pages/editor/file-upload.component';
import { PasswordEditorComponent }   from './components/account/password-editor.component';
import { AppCommunicateService } from './core/services/app-communicate.service';
import { HelpComponent }   from './components/help/help.component';
import { HelpRuComponent }   from './components/help/help-ru.component';
import { HelpEnComponent }   from './components/help/help-en.component';
import { MyPagesComponent }   from './components/pages/my-pages.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { CategoriesComponent } from './components/categories/categories.component';
import { NewsComponent } from './components/news/news.component';
import { SiteNewsComponent } from './components/facade/site-news.component';
import { TermsConditionsComponent } from './components/facade/terms-conditions.component';
import { ContactUsComponent } from './components/facade/contact-us.component';
import { PrivacyPolicyComponent } from './components/facade/privacy-policy.component';
import { PageCreateComponent } from './components/pages/editor/page-create.component';
import { SiteNewsEnComponent } from './components/facade/site-news-en.component';
import { SiteNewsRuComponent } from './components/facade/site-news-ru.component';
import { WaitComponent } from './components/messages/wait.component';
import { NewsSourcesComponent } from './components/news/news-sources.component';

//import { ModalModule } from 'ng2-bs4-modal/ng2-bs4-modal';
import { ModalModule } from './core/modal/ng2-bs4-modal';
import { Config } from './app.config';


class AppBaseRequestOptions extends BaseRequestOptions {
    headers: Headers = new Headers();

    constructor() {
        super();
        this.headers.append('Content-Type', 'application/json');
        this.body = '';
    }
}


export function createTranslateLoader(http: Http) {
    return new TranslateStaticLoader(http, './i18n', '.json');
}



@NgModule({
    imports: [
        BrowserModule,
        HttpModule,
        TranslateModule.forRoot({
            provide: TranslateLoader,
            useFactory: (createTranslateLoader),
            deps: [Http] 
        }),
        JsonpModule,
        FormsModule,
        routing,
        ModalModule
    ],

    declarations: [
        AppComponent, 
        HomeComponent, 
        UnderConstructionComponent,
        PageComponent,
        ColumnComponent,
        RowComponent,
        LinkComponent,
        PagesListComponent,
        HideIfFalsePipe,
        PageEditorComponent,
        MessengerComponent,
        DialogButtonComponent,
        ColumnEditorComponent,
        RowListEditorComponent,
        LinkListEditorComponent,
        LinkEditorComponent,
        RowEditorComponent,
        ColumnListEditorComponent,
        NewsSourceSelectorComponent,
        SidebarComponent,
        LoginComponent,
        RegisterComponent,
        NotFoundComponent,
        MyPagesEditorComponent,
        ColumnPresenterComponent,
        RowPresenterComponent,
        LinkPresenterComponent,
        SpaceValidatorDirective,
        UrlStartValidatorDirective,
        FileUploadComponent,
        AccountComponent,
        PasswordEditorComponent,
        HelpComponent,
        HelpRuComponent,
        HelpEnComponent,
        MyPagesComponent,
        CategoriesComponent,
        NewsComponent,
        SiteNewsComponent,
        TermsConditionsComponent,
        ContactUsComponent,
        PrivacyPolicyComponent,
        PageCreateComponent,
        SiteNewsEnComponent,
        SiteNewsRuComponent,
        WaitComponent,
        NewsSourcesComponent
    ],

    providers: [
        DataService,
        UtilityService,
        MessengerService,
        AccountService,
        Config,
        AppCommunicateService//,
        //{ provide: LocationStrategy, useClass: HashLocationStrategy }
        //{ provide: RequestOptions, useClass: AppBaseRequestOptions }
    ],

    bootstrap: [AppComponent]
})

export class AppModule { }
