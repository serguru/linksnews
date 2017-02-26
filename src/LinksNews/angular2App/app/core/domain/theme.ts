import * as _ from 'lodash';

export class Theme {
    id: number;
    name: string;

    clone(): Theme {
        return <Theme>_.assign(new Theme(), this);
    }

    static from (theme: Theme): Theme {
        let result: Theme = new Theme();
        _.assign(result, theme);
        return result;
    }
}
