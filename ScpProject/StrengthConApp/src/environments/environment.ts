// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  endpointURL: 'http://localhost:55653',
  userCookieSettingName: 'userToken',
  userCookieCoachName: 'isCoach',
  userCookieDomain: '',
  userCookieName: 'name',
  stripeKey: 'pk_test_4LWAo6NQ9c5ty6y0Tz63dhjZ00eX6FlRDY',
  userCookieFullName: 'fullName',
  userCookieEmail: 'Email',
  userCookieWeightRoom: 'weightRoom',
  userCookieRoles: 'Roles',
  userCookieIsHeadCoach: 'isheadCoach',
  //this is set to false on initial creation of Organization. Once the user completes the stripe checkout and puts in their credit card
  //this will be set to true. Once Set to TRUE it will always be true. There will be other flags for their status as a customer
  userCookieIsCustomer: 'isCustomer',
  userCookieBadCreditCard: 'badCreditCard',
  userCookieSubscriptionEnded: 'subEnded',
  userCookieCreditCardExpiring: 'ccExpire',
  signalREndPoint: 'https://strengthcoachprosignalr.azurewebsites.net/api/'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
