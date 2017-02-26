import * as _ from 'lodash';
import { Page } from "./page/page";

export class ContentCategory {
    id: number;
    name: string;
    description: string;
    selected: boolean;

    pagesCount: number;
    authorsCount: number;
    newsSourcesCount: number;

    pages: Array<Page>;

    clone(): ContentCategory {
        let result: ContentCategory = <ContentCategory>_.assign(new ContentCategory(), this);
        return result;
    }

    static from (category: ContentCategory): ContentCategory {
        let result: ContentCategory = new ContentCategory();
        _.assign(result, category);
        return result;
    }
}