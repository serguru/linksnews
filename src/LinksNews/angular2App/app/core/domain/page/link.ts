import { ViewModes } from "../../common/enums";
import * as _ from 'lodash';

export class Link {
    id: number;
    columnId: number;
    linkIndex: number;
    title: string = "New Link";
    description: string = "New Link Description";
    imageUrl: string;
    hint: string;
    href: string = "http://#";
    buttonAccess: boolean;
    buttonTitle: string;
    buttonIndex: number;
    buttonImageUrl: string;
    showImage: boolean = true;
    showDescription: boolean = true;
    showAd: boolean = false;
    adContent: string;
    newsLink: boolean = false;
    viewModeId: ViewModes = ViewModes.List;
    selected: boolean = false;

    clone(): Link {
        let result: Link = <Link>_.assign(new Link(), this);
        return result;
    }

    static from(link: Link): Link {
        let result: Link = new Link();
        _.assign(result, link);
        return result;
    }
}
