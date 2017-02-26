import * as _ from 'lodash';
import { Link } from "./page/link";

export class NewsSource {
    id: number;

    newsProviderId: number;
    newsProvider: string;

    newsSourceId: string;
    newsSourceDescription: string;
    newsSourceUrl: string;

    newsSourceSmallLogoUrl: string;
    newsSourceMediumLogoUrl: string;
    newsSourceLargeLogoUrl: string;
 
    contentCategoryId: number;
    contentCategory: string;
    contentCategoryTranslated: string;
 
    countryId: number;
    countryCode: string;
    countryName: string;
    countryNameTranslated: string;
 
    languageId: number;
    languageCode: string;
    languageName: string;
    languageNameTranslated: string;
        
    selected: boolean = false;

    links: Array<Link>;


    clone(): NewsSource {
        let result: NewsSource = <NewsSource>_.assign(new NewsSource(), this);
        return result;
    }

    static from (newsSource: NewsSource): NewsSource {
        
        let result: NewsSource = new NewsSource();
        _.assign(result, newsSource);
        return result;
    }
}