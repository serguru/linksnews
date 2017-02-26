import { ModuleWithProviders }  from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/facade/home.component';
import { PageComponent } from './components/pages/page.component';
import { PagesListComponent } from './components/pages/pages-list.component';
import { LoginComponent } from './components/account/login.component';
import { RegisterComponent } from './components/account/register.component';
import { NotFoundComponent } from './components/not-found.component';
import { AccountComponent } from './components/account/account.component';
import { HelpComponent } from './components/help/help.component';
import { MyPagesComponent } from './components/pages/my-pages.component';
import { CategoriesComponent } from './components/categories/categories.component';
import { NewsComponent } from './components/news/news.component';
import { SiteNewsComponent } from './components/facade/site-news.component';
import { TermsConditionsComponent } from './components/facade/terms-conditions.component';
import { ContactUsComponent } from './components/facade/contact-us.component';
import { PrivacyPolicyComponent } from './components/facade/privacy-policy.component';

const appRoutes: Routes = [
    {
        path: '',
        redirectTo: '/home',
        pathMatch: 'full'
    },
    {
        path: 'home',
        component: HomeComponent
    },
    {
        path: 'page/:login/:page',
        component: PageComponent
    },
    //{
    //    path: 'categories',
    //    component: CategoriesComponent
    //},
    {
        path: 'categories/:category',
        component: CategoriesComponent
    },
    //{
    //    path: 'news',
    //    component: NewsComponent
    //},
    {
        path: 'news/:sourceId',
        component: NewsComponent
    },
    {
        path: 'myPages',
        component: MyPagesComponent
    },
    {
        path: 'account',
        component: AccountComponent
    },
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'register',
        component: RegisterComponent
    },
    {
        path: 'help',
        component: HelpComponent
    },
    {
        path: 'siteNews',
        component: SiteNewsComponent
    },
    {
        path: 'termsConditions',
        component: TermsConditionsComponent
    },
    {
        path: 'contactUs',
        component: ContactUsComponent
    },
    {
        path: 'privacyPolicy',
        component: PrivacyPolicyComponent
    },
    {
        path: '404',
        component: NotFoundComponent
    },
    {
        path: '**',
        redirectTo: '404'
    }
];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);

/*
{path: '/home/...', name: 'Home', component: HomeComponent}
{path: '/', redirectTo: ['Home']},
{path: '/user/...', name: 'User', component: UserComponent},
{path: '/404', name: 'NotFound', component: NotFoundComponent},

{path: '/*path', redirectTo: ['NotFound']}
*/