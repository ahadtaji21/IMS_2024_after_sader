export interface SfipPlan {
	id: number;
	code: string;
	startYear: Date;
	endYear: Date;
	// children: SfipPrioritySet[];
	// category: string;
	// categoryCode: string;
	// title: string;
	// itemKey: string;
}
export interface SfipPrioritySet {
	id: number;
	name: string;
	containingPlans: number[];
	// children: SfipPriority[];
	// category: string;
	// categoryCode: string;
	// title: string;
	// itemKey: string;
}
export interface SfipPriority {
	id: number;
	code: string;
	name: string;
	formulation: string;
	prioritySetId: number;
	// children: SfipGoal[];
	// category: string;
	// categoryCode: string;
	// title: string;
	// itemKey: string;
}
export interface SfipGoal {
	id: number;
	code: string;
	formulation: string;
	priorityId: number;
	// children: SfipObjective[];
	// category: string;
	// categoryCode: string;
	// title: string;
	// itemKey: string;
}
export interface SfipObjective {
	id: number;
	code: string;
	formulation: string;
	goalId: number;
	// children: SfipIndicator[];
	// category: string;
	// categoryCode: string;
	// title: string;
	// itemKey: string;
}
export interface SfipIndicator {
	id: number;
	code: string;
	formulationHtml: string;
	objectiveId: number;
	adherance: SfipIndicatorAdheranceItem[];
	// category: string;
	// categoryCode: string;
	// title: string;
	// itemKey: string;
}
export interface SfipIndicatorAdheranceItem {
	indicatorId: number;
	officeCode: string;
	hasAdhered: boolean;
	// itemKey: string;
}

export enum SfipItemCategory {
	Plan = "PLAN",
	PrioritySet = "PRIORITY_SET",
	Priority = "PRIORITY",
	Goal = "GOAL",
	Objective = "OBJECTIVE",
	Indicator = "INDICATOR",
	Activity = "ACTIVITY"
}

export interface SfipTreeItem {
	itemKey: string;
	title: string;
	categoryCode: SfipItemCategory;
	category: string;
	children: SfipTreeItem[];
	itemData: any;
}