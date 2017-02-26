import { Injectable } from '@angular/core';
import {Router} from '@angular/router';
import { Page } from "../domain/page/page";
import { Row } from "../domain/page/row";
import { Link } from "../domain/page/link";
import { Column } from "../domain/page/column";
import { Account } from "../domain/account";
//import * as _ from 'lodash';  
import { ColumnTypes } from "../common/enums";
import { ViewModes } from "../common/enums";
import { TranslateService, LangChangeEvent } from 'ng2-translate/ng2-translate';

@Injectable()
export class UtilityService {

    static patterns = [
        
        
        ]

    public router: Router;
    public translate: TranslateService 

    constructor(router: Router, translate: TranslateService) {
        this.router = router;
        this.translate = translate;
    }

    navigateToLogin() {
        this.router.navigate(["/login"]);
    }

    viewModeValid(value: any): boolean {
        if (!value) {
            return false;
        }
        let result: boolean = value == ViewModes.List || value == ViewModes.Tile; 
        return result;
    }

    loginToEllipsis(value: string): string {
        if (!value) {
            return value;
        }

        if (value.length <= 23) {
            return value;
        }
        
        return value.substr(0, 20) + "...";
    }


    getCookie(name) {
        let myWindow: any = window;
        name = myWindow.escape(name);
        let regexp = new RegExp('(?:^' + name + '|;\\s*' + name + ')=(.*?)(?:;|$)', 'g');
        let result = regexp.exec(document.cookie);
        return (result === null) ? null : myWindow.unescape(result[1]);
    };
}