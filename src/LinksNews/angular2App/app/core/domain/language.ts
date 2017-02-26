import * as _ from 'lodash';

export class Language {
    id: number;
    code: string;
    name: string;
    supportedByInterface: boolean;
    supportedByNews: boolean;

    clone(): Language {
        return <Language>_.assign(new Language(), this);
    }

    static from (language: Language): Language {
        let result: Language = new Language();
        _.assign(result, language);
        return result;
    }
}
