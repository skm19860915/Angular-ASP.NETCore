

export const environment = {
  production: true,
  endpointURL: 'https://api.strengthcoachpro.com',
  userCookieSettingName: 'userToken',
  userCookieDomain: 'strengthcoachpro.com',
  userCookieCoachName: 'isCoach',
  userCookieName: 'name',
  stripeKey: 'pk_live_aDT1t4dYBm0rxzjhgxIlEoci00Lo17T0jT',
  userCookieFullName: 'fullName',
  userCookieEmail: 'Email',
  userCookieWeightRoom: 'weightRoom',
  userCookieRoles: 'Roles',
  userCookieIsHeadCoach: 'isheadCoach',
    //this is set to false on initial creation of Organization. Once the user completes the stripe checkout and puts in their credit card
  //this will be set to true. Once Set to TRUE it will always be true. There will be other flags for their status as a customer
  userCookieIsCustomer : 'isCustomer',
  userCookieBadCreditCard : 'badCreditCard',
  userCookieSubscriptionEnded : 'subEnded',
  userCookieCreditCardExpiring: 'ccExpire',
  signalREndPoint : 'https://strengthcoachprosignalr.azurewebsites.net/api/'
};

