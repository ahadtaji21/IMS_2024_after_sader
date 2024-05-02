declare module 'url' {
    interface Url {
        // query: any;
        search: string | null;
        parse(url: any, parseQueryString: any, slashesDenoteHost?: any): Url;
        format(): string;
    }
    function parse(url: any, parseQueryString: any, slashesDenoteHost?: any): Url;
    function format(obj: Url | string): string;
}
