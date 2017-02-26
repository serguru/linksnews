import { Column } from "./column";
import { Account } from "../account";
import * as _ from 'lodash';
import { ContentCategory } from "../content-category";

export class Page {
    id: number;
    accountId: number;
    name: string;
    title: string;
    publicAccess: boolean = false;
    imageUrl: string;
    description: string;
    pageIndex: number;
    showTitle: boolean = true;
    showDescription: boolean = true;
    showImage: boolean = true;
    showAd: boolean = false;
    adContent: string;
    columns: Array<Column> = new Array<Column>();
    login: string;
    selected: boolean = false;
    categories: Array<ContentCategory> = new Array<ContentCategory>();

    get checkedCategoriesCount(): number {
        if (!this.categories) {
            return 0;
        }
        let i: number = 0;
        this.categories.forEach(x => {
            if (x.selected) {
                i++;
            }
        });    
        return i;
    }

    clone(): Page {

        let result: Page = <Page>_.assign(new Page(), this);

        if (this.columns) {
            result.columns = new Array<Column>();
            this.columns.forEach(x => result.columns.push(x.clone()));
        }

        if (this.categories) {
            result.categories = new Array<ContentCategory>();
            this.categories.forEach(x => result.categories.push(x.clone()));
        }

    
        return result;
    }

    cloneNoColumnsChildren(): Page {

        let result: Page = <Page>_.assign(new Page(), this);

        if (this.columns) {
            result.columns = new Array<Column>();
            this.columns.forEach(x => result.columns.push(x.cloneNoChildren()));
        }

        if (this.categories) {
            result.categories = new Array<ContentCategory>();
            this.categories.forEach(x => result.categories.push(x.clone()));
        }
    
        return result;
    }

    static from(page: Page): Page {

        let result: Page = new Page();

        _.assign(result, page);

        result.columns = new Array<Column>();
        
        if (page.columns) {
            page.columns.forEach(x => result.columns.push(Column.from(x)));
        }

        result.categories = new Array<ContentCategory>();

        if (page.categories) {
            page.categories.forEach(x => result.categories.push(ContentCategory.from(x)));
        }

        return result;
    }

}