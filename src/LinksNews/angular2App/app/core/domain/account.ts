import { Language } from "./language";
import { Theme } from "./theme";
import * as _ from 'lodash';

export class Account {
    id: number;
    roleId: number;
    login: string;
    //password: string;
    firstName: string;
    lastName: string;
    languageId: number;
    email: string;
    address: string;
    telephone: string;
    website: string;
    comment: string;
    themeId: number;
    locked: boolean;
    dateCreated: Date;
    imageUrl: string;
    newsRefreshInterval: number;

    clone(): Account {
        let result: Account = <Account>_.assign(new Account(), this);
        return result;
    }

    static from (account: Account): Account {
        
        let result: Account = new Account();
        _.assign(result, account);
        return result;
    }
}
