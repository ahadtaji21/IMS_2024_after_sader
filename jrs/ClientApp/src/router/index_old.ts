import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import { ImsUser, User } from '@/axiosapi';
import { i18n } from '../i18n';
import store from '../store/index';
import * as msal from "@azure/msal-browser";

import Home from '../views/Home.vue'
import { AuthModule } from '@/services/AuthModule';

Vue.use(VueRouter)

const routes: Array<RouteConfig> = [
  {
    path: '/',
    redirect: `/${i18n.locale}/home`
  },
  {
    // path: '/:code(^code=.+)?',
    path: '/:code(code=.+)?',
    name: 'msalRedirect',
    // redirect: `/${i18n.locale}/home`
    redirect: to => ({
      path: `/${i18n.locale}/home`,
      query: { code: to.params.code.replace(/^(code=)/,"") }
    })
  },
  {
    path: '/:lang(en|fr)',
    component: {
      render (c:any) { return c('router-view') }
    },
    children: [
      {
        path: 'playground',
        name: 'playground',
        component: () => import('../views/Playground.vue')
      },
      {
          path: 'coi',
          name: 'coi',
          /*component: () => import('../views/CategoryOfIntervention.vue')*/
          component: () => import('../views/Pms/Config/PmsCOI.vue')
      },
      {
        path: 'department',
        name: 'department',
        /*component: () => import('../views/CategoryOfIntervention.vue')*/
        component: () => import('../views/Pms/Config/PmsDepartment.vue')
    },
      {
        path: 'home',
        name: 'home',
        component: Home
      },
      {
        path: 'about',
        name: 'about',
        // route level code-splitting
        // this generates a separate chunk (about.[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        component: () => import(/* webpackChunkName: "about" */ '../views/About.vue')
      },
      {
        path: 'helpdesk',
        name: 'helpdesk',
        beforeEnter() {
          window.open("https://helpdesk.jrs.net/help/3882375463",'_blank')
        }
      },
      {
        path: 'timesheet',
        name: 'timesheet',
        // route level code-splitting
        // this generates a separate chunk (about.[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        component: () => import('../views/TimesheetCalendar.vue')
      },
      {
        path: 'data-table/:table_name',
        name: 'data table',
        component: () => import('../views/JrsSimpleView.vue')
      },
      {
        path: 'type-of-service',
        name: 'Type of Service',
        component: () => import('../views/Pms/Config/PmsTOS.vue')
      },
      {
        path: 'ims-indicator',
        name: 'Indicator',
        component: () => import('../views/Ind/ImsIndicator.vue')
      },
      {
        path: 'gmt-donor-account',
        name: 'GMT Donor Account',
        component: () => import('../views/Gmt/Donors/GmtDonorAccount.vue')
      },
      {
        path: 'jrs-chart-of-accounts',
        name: 'JRS Chart Of Accounts',
        component: () => import('../views/Pms/JRSChartOfAccounts.vue')
      },
      {
        path: 'global-indicators',
        name: 'Global Indicators',
        component: () => import('../views/GlobalIndicator.vue')
      },
      {
        path: 'table-definition',
        name: 'Table definition',
        component: () => import('../views/ImsTableDefinition.vue')
      },
      {
        path: 'project-code',
        name: 'Project Code',
        component: () => import('../views/Pms/PmsProjectCodeList.vue')
      },
      {
        path: 'project-narrative',
        name: 'Project Narrative',
        component: () => import('../views/PmsProjectNarrative.vue')
      },
      {
        path: 'annual-plan-list',
        name: 'Annual Plan List',
        component: () => import('../views/Pms/PmsAnnualPlanList.vue')
      },
      {
        path: 'annual-plan-list-versioned',
        name: 'Versioned Annual Plan List',
        component: () => import('../views/Pms/PmsAnnualPlanListVersioned.vue')
      },
      {
        path: 'grant-list',
        name: 'Grant List',
        component: () => import('../views/Gmt/Grants/GmtGrantList.vue')
      },
      {
        path: 'recovery-deleted-grants',
        name: 'Recovery Deleted Grants',
        component: () => import('../views/Gmt/Grants/GmtDeletedGrantList.vue')
      },
      {
        path: 'annual-plan-details',
        name: 'Annual Plan Details',
        component: () => import('../views/Pms/PmsAnnualPlanDetails.vue'),
        props:true
      },
      {
        path: 'gmt-annual-plan-details',
        name: 'GMT Annual Plan Details',
        component: () => import('../views/Pms/PmsAnnualPlanDetails.vue'),
        props:true
      },
      {
        path: 'imp-annual-plan-details',
        name: 'IMP Annual Plan Details',
        component: () => import('../views/Pms/PmsAnnualPlanDetails.vue'),
        props:true
      },
      {
        path: 'gmt-annual-plan-search-gap',
        name: 'GMT Annual Plan Search GAp',
        component: () => import('../views/Gmt/AnnualPlan/GmtAnnualPlanSearchGAP.vue'),
        props:true
      },
      {
        path: 'gmt-donor-details',
        name: 'Donor Details',
        component: () => import('../views/Gmt/Donors/GmtDonorDetails.vue'),
        props:true
      },
      {
      path: 'gmt-grant-details',
        name: 'Grant Details',
        component: () => import('../views/Gmt/Grants/GmtGrantDetails.vue'),
        props:true
      },
      {
        path: 'gmt-donor-list',
        name: 'Donors List',
        component: () => import('../views/Gmt/Donors/GmtDonorList.vue'),
        props:true
      },
      {
        path: 'biodata-list',
        name: 'biodata list',
        component: () => import('../views/Hrm/HrmBiodata.vue')
      },
      {
        path: 'biodata-list-deleted',
        name: 'biodata list deleted',
        component: () => import('../views/Hrm/HrmBiodataDeleted.vue')
      },
      {
        path: 'admin-area',
        name: 'adminnistrative areas',
        component: () => import('../views/ImsAdminArea.vue')
      },
      {
        path: 'hr-department-config',
        name: 'department config',
        component: () => import ('../views/Hrm/HrmDepartment.vue')
      },
      {
        path: 'contact',
        name: 'contact config',
        component: () => import ('../views/Hrm/HrmContact.vue')
      },
      {
        path: 'status',
        name: 'status config',
        component: () => import ('../views/ImsStatus.vue')
      },
      {
        path: 'vacancy',
        name: 'vacancy config',
        component: () => import ('../views/Hrm/HrmVacancy.vue')
      },
      {
        path: 'agreement-list',
        name: 'agreement list',
        component: () => import('../views/Hrm/HrmAgreement.vue')
      },
      {
        path: 'household-list',
        name: 'household list',
        component: () => import('../views/Hrm/HrmHousehold.vue')
      },
      {
        path: 'family-list',
        name: 'family list',
        component: () => import('../views/Hrm/HrmFamily.vue')
      },

      {
        path: 'office-list',
        name: 'office list',
        component: () => import('../views/Hrm/HrmOffice.vue')
      },
      {
        path: 'activity-register',
        name: 'Register activity',
        component: () => import ('../views/Pms/PmsActivityInstanceRegister.vue')
      },
      {
        path: 'placement-test',
        name: 'Placement Test',
        component: () => import ('../views/Pms/PmsPlacementTest.vue')
      },
      {
        path: 'exam',
        name: 'Exam',
        component: () => import ('../views/Pms/PmsExam.vue')
      },
      {
        path: 'evaluation',
        name: 'Evaluation',
        component: () => import ('../views/Pms/PmsEvaluation.vue')
      },
      {
        path: 'personal-area',
        name: 'personalArea',
        component: () => import ('../views/PersonalArea.vue')
      },
      {
        path: 'supervisor-timesheet',
        name: 'supervisor timesheet',
        component: () => import('../views/Hrm/Supervisor/HrmTimesheetSupervisor.vue')
      },
      {
        path: 'template-config',
        name: 'Template configuration',
        component: () => import('../views/ImsTemplateConfig.vue')
      },
      {
        path: 'ap-config',
        name: 'Annual Plan Configuration',
        component: () => import('../views/Pms/PmsApConfig.vue')
      },
      {
        path: 'template-definition',
        name: 'Template definition',
        component: () => import('../views/ImsTemplateDef.vue')
      },
      {
        path: 'template-doc-generator',
        name: 'Template document generator',
        component: () => import('../views/ImsTemplateDocGenerator.vue')
      },
      /*{
        path: 'supervisor-leave-requests',
        name: 'supervisor leave requests',
        component: () => import('../views/Hrm/Supervisor/HrmLeaveRequestSupervisor.vue')
      },*/
      {
        path: 'timesheet-type-day',
        name: 'timesheet type day config',
        component: () => import('../views/Hrm/HrmTimesheet.vue')
      },
      {
        path: 'education-and-skills',
        name: 'education and skills config',
        component: () => import('../views/Hrm/HrmEducationAndSkills.vue')
      },
      {
        path: 'position-config',
        name: 'position config',
        component: () => import('../views/Hrm/HrmPosition.vue')
      },
      {
        path: 'classes-list',
        name: 'classes list',
        component: () => import('../views/Pms/PmsClassesList.vue')
      },
      {
        path: 'classes-attendance-process-instance',
        name: 'classes attendance',
        component: () => import('../views/Pms/PmsClassesAttendanceProcessInstance.vue')
      },
      {
        path: 'beneficiary-list',
        name: 'beneficiaryList',
        component: () => import('../views/Pms/PmsBeneficiary.vue')
      },
      {
        path: 'training-form',
        name: 'trainingForm',
        component: () => import('../views/Pms/PmsTrainingForm.vue'),
        props:true
      },
      {
        path: 'training-form-list',
        name: 'trainingFormList',
        component: () => import('../views/Pms/PmsTrainingFormList.vue')
      },
      {
        path: 'case-management',
        name: 'caseManagement',
        component: () => import('../views/Pms/PmsCaseManagement.vue')
      },
      {
        path: 'case-management-list',
        name: 'caseManagementList',
        component: () => import('../views/Pms/PmsCaseManagementList.vue')
      },
      {
        path: 'distribution',
        name: 'distribution',
        component: () => import('../views/Pms/PmsDistribution.vue')
      },
      {
        path: 'settlements-biodata',
        name: 'settlements biodata',
        component: () => import('../views/Set/SetSettlementsBiodata.vue')
      },
      {
        path: 'questionnaire-config',
        name: 'questionnaireConfig',
        component: () => import('../views/ImsQuestionnaireConfig.vue')
      },
      {
        path: 'settl-beneficiary',
        name: 'settlements detained person',
        component: () => import('../views/Pms/PmsBeneficiary.vue')
      },
      {
        path: 'questionnaire-fill-in',
        name: 'questionnaireFillIn',
        component: () => import('../views/ImsQuestionnaireCompilation.vue')
      },
      {
        path: 'questionnaire-review',
        name: 'questionnaireReview',
        component: () => import('../views/ImsQuestionnaireReview.vue'),
        props: route => ({ questionnaireCode: route.query.q})
      },
      {
        path: 'questionnaire-review/:questionnaire_code',
        name: 'questionnaireReviewbyCode',
        component: () => import('../views/ImsQuestionnaireReview.vue'),
        props: route => ({ questionnaireCode: route.params.questionnaire_code})
      },
      {
        path: 'settlements-visitor-overview',
        name: 'settlementVisitorOverview',
        component: () => import('../views/Set/setSettlementVisitorOverview.vue')
      },
      {
        path: 'settlements-detained-person-stories',
        name: 'settlements detained person stories',
        component: () => import('../views/Set/SetDetainedPersonStories.vue')
      },
      {
        path: 'legal-frameworks',
        name: 'legalFrameworks',
        component: () => import('../views/Set/SetLegalFramework.vue')
      },
      {
        path: 'settlements-bio-visits-summary',
        name: 'settlementBioVisitsSummary',
        component: () => import('../views/Set/setSettlementVisitsSummary.vue')
      },
      {
        path: 'settle-position',
        name: 'settlements position',
        component: () => import('../views/Hrm/HrmPosition.vue')
      },
      {
        path: 'settl-biodata-list',
        name: 'settlements biodata list',
        component: () => import('../views/ImsAdminArea.vue')
      },
      {
        path: 'sfip-indicator-dashboard',
        name: 'sfip global indicator dashboard',
        component: () => import('../views/SfipDashboardIndicator.vue')
      },
      {
        path: 'sfip-plan-design',
        name: 'sfipPlanDesign',
        component: () => import('../views/Sfip/SfipPlanDesign.vue')
      },
      {
        path: 'sfip-activity-definition',
        name: 'sfipActivityDefinition',
        component: () => import('../views/Sfip/SfipActivityDefinition.vue')
      },
      {
        path: 'sfip-indicator-achievement',
        name: 'sfipIndicatorAchievement',
        component: () => import('../views/Sfip/SfipIndicatorAchievement.vue')
      },
      {
        path: 'sfip-plan-overview',
        name: 'sfipPlanOverview',
        component: () => import('../views/Sfip/SfipPlanOverview.vue')
      },
      {
        path: 'sfip-activity-overview',
        name: 'sfipActivityOverview',
        component: () => import('../views/Sfip/SfipActivityOverview.vue')
      },
      
      {
        path: 'individual',
        name: 'individual',
        component: () => import('../views/Individuals/Home/IndividualHome.vue')
      },
      {
        path: 'individualAdd',
        name: 'individualAdd',
        component: () => import('../views/Individuals/Add/IndividualAdd.vue')
      },
      {
        path: 'individualEdit/:id',
        name: 'individualEdit',
        component: () => import('../views/Individuals/Edit/IndividualEdit.vue'),
        props: true
      },
      {
        path: 'donnor',
        name: 'donnor',
        component: () => import('../views/Gmt/Donors/New/List/List.vue')
      },
      {
        path: 'Staff',
        name: 'Staff',
        component: () => import('../views/Hrm/Staff/New/List/List.vue')
      },
      {
        path: 'donnorAdd',
        name: 'donnorAdd',
        component: () => import('../views/Gmt/Donors/New/Create/Create.vue'),
      },
      {
        path: 'donnorEdit/:id',
        name: 'donnorEdit',
        component: () => import('../views/Gmt/Donors/New/Edit/Edit.vue'),
        props: true
      },

      
    ]
  },
  {
    path: '*',
    name: '404',
    component: () => import('../views/404.vue'),
    props: route => ({ messageKey: route.query.mk, })
  }
];

const router = new VueRouter({
  // mode: 'history',
  base: process.env.BASE_URL,
  routes
})

//--- Manage Navigation Guards ---//
router.beforeEach((to:any, from:any, next:Function) => {

  // Login Jrs user into IMS system. Subsequently load user menu and check destination route.
  const loginJrsUser:Function = (username:string)=>{
    const app:any = router.app;
    const myMSALObj:any = app.$msal;
    store.dispatch('auth/loginAzure', { userName: username , MsalObject:myMSALObj})
    .then( (res:any) => {
      setupMenu(to)                                                    // Setup the user menus and continue to route
      .then( (flatMenus:any) => {
        // Get list of user available manus.
        let destinationMenuIndex = flatMenus.findIndex((menuItem: any) => menuItem.path == to.path);
        let destinationMenu = flatMenus.find((menuItem: any) => menuItem.path == to.path);
        // If the destination route is not in the list of user menus --> redirect to 404
        if (to.path != `/${i18n.locale}/404`&& destinationMenuIndex == -1) {
          next(`/${i18n.locale}/404`);
        } 

        // set active module (current)  
        setupCurrentModule(destinationMenu);

        // Complete navigation
        next();
      });
    })
    .catch( (err) => {
      console.log(err);
    });
  }
  
  // Get reference to the main MSALObj instance
  const app:any = router.app;
  const myMSALObj:any = app.$msal;

  // Handle redirect page load through MSAL 
  myMSALObj.loadAuthModule()
  .then( (resp:any) => {
    // Get logged account
    const currAccount:any = myMSALObj.account;
  
    // If no account is logged, then redirect to MSAL login
    if(!currAccount){
      myMSALObj.login('loginRedirect');
    }
  })
  .then(() => {
    // Get logged account
    const currAccount:any = myMSALObj.account;

    // If no account is logged then terminate function
    if(!currAccount){
      return;
    }

    // JRS IMS login
    let userIsLogged:Boolean = store.getters['auth/isLoggedIn'];
    if(!userIsLogged){
      loginJrsUser(currAccount.username);
    }else{
      // Complete navigation
      next();
    }
  })
  .catch((err:any) => {
    console.log(err);
    next();
  });

});

// function setLocaleFromPath(to:any, from:any){
//   let language = to.params.lang != undefined ? to.params.lang : i18n.fallbackLocale;

//   return store.dispatch('setUserLocale', language);
// }

/**
 * Setup the users menu and translate labels based on URL
 * @param to - destination route
 */
async function setupMenu(to:any){
  let language = to.params.lang != undefined ? to.params.lang : i18n.fallbackLocale;                      // Get current language (From url or default)

  let user:User = store.getters['auth/loggedUser'];                                                       // Get User
  let userUid:string|null = user!=null && user.userUid ? user.userUid : null;                             // Get User UID

  if(userUid != null){
    return store.dispatch('setUserMenu', userUid)                                                         // Set User menu
      .then(res => {
        return store.dispatch('setUserLocale', language);                                                 // Set user local
      })
      .then(res => { return store.dispatch('caclculateFlatMenu') })                                       // Calculate flat menu
    }
  else{
    return new Promise( (resolve, reject) => {
      reject('Error: userUid not found.')
    })
  }
}


/**
 * Setup the current module 
 * @param currentMenu - current menu
 */
 async function setupCurrentModule(currentMenu:any){
  let moduleLists:Array<any> = store.getters['getUserModules'];                                                // Get List of moduels for user
  if(currentMenu.moduleId){
     let current_module = moduleLists.find((ele:any)=> ele.moduleId == currentMenu.moduleId )
      //context.commit('SET_ACTIVE_MODULE', mod); )
     store.dispatch("setActiveModule",current_module)
  }
}



export default router
