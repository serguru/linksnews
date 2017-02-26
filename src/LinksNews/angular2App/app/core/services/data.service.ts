import { Injectable } from '@angular/core';
import { Http, Response, Headers } from '@angular/http';
import 'rxjs/add/operator/map';
import { Observable } from 'rxjs/Observable';
import { RegistrationData } from '../domain/registration-data';
import { Account } from '../domain/account';
import { Page } from "../domain/page/page";
import { Column } from "../domain/page/column";
import { Row } from "../domain/page/row";
import { Link } from "../domain/page/link";
import { GenericResult } from "../common/generic-result";
import { PageElement } from "../common/enums";
import { NewsSource } from '../domain/news-source';
import { Message } from "../domain/message";
import { Config } from "../../app.config";

@Injectable()
export class DataService {

    public actionUrl: string;
    public headers: Headers;

    constructor(
        public http: Http,
        public config: Config
        ) {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'application/json');
    }

    public processResponse(result: GenericResult, okCallback?: Function, errorCallback?: Function): void {
        if (result.error) {
            //if (result.message) {
            //    this.messengerService.showError("Error", result.message);
            //    return;
            //} 

            if (errorCallback) {
                errorCallback(result);
            }
            return;                    
        }

        //if (result.message) {
        //    this.messengerService.showOK("Message", result.message);
        //}

        if (okCallback) {
            okCallback(result.data);
        }
    }

    sendPostNoHeaders(url: string, data: any, okCallback?: Function, errorCallback?: Function)  {

        let fullUrl: string = this.config.serverUrl + url;

        this.http.post(fullUrl, data)
        .map(res => <any>(<Response>res).json())
        .subscribe(
            data => {
                this.processResponse(data, okCallback, errorCallback);
            },
            (error) => {
                if (errorCallback) {
                    errorCallback();
                }
            },
            () => {
            }
            );
    }

    sendPost(url: string, data: any, okCallback?: Function, errorCallback?: Function)  {
        let fullUrl: string = this.config.serverUrl + url;

        this.http.post(fullUrl, data, { headers: this.headers })
        .map(res => <any>(<Response>res).json())
        .subscribe(

            (data) => {
                this.processResponse(data, okCallback, errorCallback);
            },
            (error) => {
                if (errorCallback) {
                    errorCallback();
                }
            },
            () => {
            });
    }

    getPage(owner: string, page: string, refreshCache: boolean, okCallback: Function, errorCallback: Function) {
        let data = JSON.stringify({
            login: owner,
            page: page,
            refreshCache: refreshCache
        });
        this.sendPost("pages/getPage", data, okCallback, errorCallback);
    }

    getNewsSources(okCallback: Function, errorCallback?: Function){
        this.sendPost("/pages/newsSources", null, okCallback, errorCallback);
    }

    getPagesByCategory(categoryName: string, okCallback: Function, errorCallback: Function){
        let url: string = "/pages/category";
        let data = JSON.stringify(categoryName);
        this.sendPost(url, data, okCallback, errorCallback);
    }

    getMyPages(okCallback: Function, errorCallback: Function){
        let url: string = "/pages/myPages/";
        this.sendPost(url, null, okCallback, errorCallback);
    }

    getLinkById(id: number, okCallback: Function, errorCallback: Function) {
        this.sendPost("pages/getLinkById", id, okCallback, errorCallback);
    }

    getColumnById(id: number, okCallback: Function, errorCallback: Function) {
        this.sendPost("pages/getColumnById", id, okCallback, errorCallback);
    }

    getRowById(id: number, okCallback: Function, errorCallback: Function) {
        this.sendPost("pages/getRowById", id, okCallback, errorCallback);
    }

    saveColumn(column: Column, rows2save: Array<any>, links2save: Array<any>, okCallback: Function, errorCallback?: Function) {
        let data = {
            column: column,
            rows2save: rows2save,
            links2save: links2save
        }
        this.sendPost("pages/saveColumn", JSON.stringify(data), okCallback, errorCallback);
    }

    saveRow(row: Row, columns2save: Array<any>, okCallback: Function, errorCallback: Function) {
        let data = {
            row: row,
            columns2save: columns2save
        }
        this.sendPost("pages/saveRow", JSON.stringify(data), okCallback, errorCallback);
    }

    savePage(page: Page, columns2save: Array<any>, okCallback: Function, erorCallback: Function) {
        let data = {
            page: page,
            columns2save: columns2save
        }

        this.sendPost("pages/savePage", JSON.stringify(data), okCallback, erorCallback);
    }

    savePagesList(pagesList: Array<any>, okCallback: Function, erorCallback: Function) {
        let data = JSON.stringify(pagesList);
        this.sendPost("pages/savePagesList", data, okCallback, erorCallback);
    }

    uploadImage4Link(linkId: number, fileToUpload: any, okCallback: Function, erorCallback: Function) {
        let fileName: string = String(linkId);
        let input = new FormData();
        input.append("file", fileToUpload, fileName);
        this.sendPostNoHeaders("pages/uploadImage4Link", input, okCallback, erorCallback);
    }

    deleteImage4Link(linkId: number, okCallback: Function, erorCallback: Function) {
        this.sendPost("pages/deleteImage4Link", linkId, okCallback, erorCallback);
    }

    deleteImage4Page(pageId: number, okCallback: Function, errorCallback: Function) {
        this.sendPost("pages/deleteImage4Page", pageId, okCallback, errorCallback);
    }

    deleteImage4Account(login: string, okCallback: Function, errorCallback: Function) {
        let data = JSON.stringify(login);
        this.sendPost("account/deleteImage4Account", data, okCallback, errorCallback);
    }

    deleteImage4Column(columnId: number, okCallback: Function, errorCallback: Function) {
        this.sendPost("pages/deleteImage4Column", columnId, okCallback, errorCallback);
    }

    uploadImage4Page(pageId: number, fileToUpload: any, okCallback: Function, errorCallback: Function) {
        let fileName: string = String(pageId);
        let input = new FormData();
        input.append("file", fileToUpload, fileName);
        this.sendPostNoHeaders("pages/uploadImage4Page", input, okCallback, errorCallback);
    }

    uploadImage4Account(login: string, fileToUpload: any, okCallback: Function, errorCallback: Function) {
        let fileName: string = String(login);
        let input = new FormData();
        input.append("file", fileToUpload, fileName);
        this.sendPostNoHeaders("account/uploadImage4Account", input, okCallback, errorCallback);
    }

    uploadImage4Column(columnId: number, fileToUpload: any, okCallback: Function, errorCallback: Function) {
        let fileName: string = String(columnId);
        let input = new FormData();
        input.append("file", fileToUpload, fileName);
        this.sendPostNoHeaders("pages/uploadImage4Column", input, okCallback, errorCallback);
    }

    getContentCategories(okCallback: Function, errorCallback: Function){
        let url: string = "/pages/contentCategories";
        this.sendPost(url, null, okCallback, errorCallback);
    }

    registerAccount(registration: RegistrationData, okCallback: Function, errorCallback: Function) {
        let data = JSON.stringify(registration);
        return this.sendPost("account/register", data, okCallback, errorCallback);
    }

    login(login: string, password: string, okCallback: Function, errorCallback: Function) {
        let data = JSON.stringify({
                login: login,
                password: password
            });
        return this.sendPost("account/login", data, okCallback, errorCallback);
    }

    logout(okCallback: Function, errorCallback: Function) {
        return this.sendPost("account/logout", null, okCallback, errorCallback);
    }

    setLanguage(language: string, errorCallback: Function) {
        let data = JSON.stringify(language);
        return this.sendPost("translate/setLanguage", data, null, errorCallback);
    }

    getInterfaceLanguages(okCallback: Function, errorCallback: Function){
        let url: string = "translate/getInterfaceLanguages";
        this.sendPost(url, null, okCallback, errorCallback);
    }

    saveLink(link: Link, okCallback: Function, errorCallback: Function) {
        let data = JSON.stringify(link);
        return this.sendPost("pages/saveLink", data, okCallback, errorCallback);
    }

    getAccount(okCallback: Function, errorCallback: Function) {
        return this.sendPost("account/account", null, okCallback, errorCallback);
    }

    saveAccount(account: Account, okCallback: Function, errorCallback: Function) {
        let data = JSON.stringify(account);
        return this.sendPost("account/saveAccount", data, okCallback, errorCallback);
    }

    savePassword(oldPassword: string, newPassword: string, login: string, 
        okCallback: Function, errorCallback?: Function) {
        let data = JSON.stringify({
            oldPassword: oldPassword,
            newPassword: newPassword,
            login: login
        });
        return this.sendPost("account/savePassword", data, okCallback, errorCallback);
    }

    deleteAccount(login: string, okCallback: Function, errorCallback: Function) {
        let data = JSON.stringify(login);
        return this.sendPost("account/deleteAccount", data, okCallback, errorCallback);
    }

    add2MyPages(pageId: number, okCallback: Function, errorCallback?: Function) {
        this.sendPost("pages/addExternalPage", pageId, okCallback, errorCallback);
    }

    copy2MyPages(pageId: number, okCallback: Function, errorCallback: Function) {
        this.sendPost("pages/copyPage", pageId, okCallback, errorCallback);
    }

    getLinksBySource(source: NewsSource, okCallback: Function, errorCallback: Function) {
        let data = JSON.stringify({
            newsSourceId: source.newsSourceId,
            newsProviderId: source.newsProviderId
        });
        this.sendPost("pages/linksBySource", data, okCallback, errorCallback);
    }

    createPage(data2save: any, okCallback: Function, errorCallback: Function) {

        let data = JSON.stringify(data2save);

        this.sendPost("pages/createPage", data, okCallback, errorCallback);
    }

    sendContactUs(message: Message, okCallback: Function, errorCallback: Function) {
        let data = JSON.stringify(message);
        this.sendPost("messages/sendContactUs", data, okCallback, errorCallback);
    }

    registerVisit(route: string, accountId: number, okCallback: Function, errorCallback: Function) {
        let data = JSON.stringify({
            route: route, 
            accountId: accountId
        });
        this.sendPost("home/registerVisit", data, okCallback, errorCallback);
    }

    removePageFromCache(login: string, pageName: string, okCallback: Function, errorCallback: Function) {
        let key = login + pageName;
        let data = JSON.stringify(key);
        this.sendPost("pages/removePageFromCache", data, okCallback, errorCallback);
    }
 
    setCookie(name: string, value: string, okCallback: Function, errorCallback: Function) {
        let data = JSON.stringify({
            name: name,
            value: value
        });
        this.sendPost("home/setCookie", data, okCallback, errorCallback);
    }
}