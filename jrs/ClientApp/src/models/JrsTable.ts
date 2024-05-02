import { RuleOperator } from "@/axiosapi";

/**
 * @param value - the name of the property in the item
 */
interface VuetifyHeader {
    text: string
    value: string
    align?: 'start' | 'center' | 'end'
    sortable?: boolean
    filterable?: boolean
    divider?: boolean
    class?: string | string[]
    width?: string | number
    filter?: (value: any, search: string, item: any) => boolean
    sort?: (a: any, b: any) => number
}


/**
 * Interface for Jrs table headers
 * 
 */
// export interface IJrsHeader extends VuetifyHeader {
export interface JrsHeader extends VuetifyHeader {
    is_pk?: boolean,
    column_name?: string,
    translationtKey?: string,
    is_hidden?: boolean,
    isCheckbox?: boolean,
    isDateTime?: boolean,
    dateTimeFormat?: 'short' | 'long',
    list_order?: number,
    prevent_truncation?: boolean,
}

export interface JrsHeaderList {
    headers: Array<JrsHeader>
}

export interface IIconAction {
    iconCode: string,
    func: Function,
    class?: string
}

/**
 * Enumeration of all possible RuleOperators. 
 */
export enum RuleOperators {
    lt = "lt",
    lte = "lt",
    eq = "eq",
    gt = "gt",
    gte = "gte",
    ne = "ne"
}

export enum Operators {
    lt = 0,
    lte = 1,
    eq = 2,
    gt = 3,
    gte = 4,
    ne = 5
}

export function codeToRuleOperator(code: string) {
    let op: RuleOperator = RuleOperator.NUMBER_5;
    switch (code) {
        case "lt":
            op = RuleOperator.NUMBER_0;
            break;
        case "lte":
            op = RuleOperator.NUMBER_1;
            break;
        case "eq":
            op = RuleOperator.NUMBER_2;
            break;
        case "gte":
            op = RuleOperator.NUMBER_3;
            break;
        case "gt":
            op = RuleOperator.NUMBER_4;
            break;
        case "ne":
            op = RuleOperator.NUMBER_5;
            break;
        default:
            throw Error("incorrect operator code");
    }
    return op;
}