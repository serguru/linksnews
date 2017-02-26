import * as _ from 'lodash';

export class Message {
    id: number;
    messageGroupId: number;
    parentMessageId: number;
    sentDate: Date;
    sentFromIP: string;
    pageId: number;
    authorAccountId: number;
    authorName: string;
    authorEmail: string;
    subject: string;
    messageText: string;

    clone(): Message {
        let result: Message = <Message>_.assign(new Message(), this);
        return result;
    }

    static from (account: Message): Message {
        let result: Message = new Message();
        _.assign(result, account);
        return result;
    }
}
