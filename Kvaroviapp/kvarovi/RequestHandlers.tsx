import AsyncStorage from '@react-native-async-storage/async-storage';
import { Platform} from 'react-native';
import * as Notifications from 'expo-notifications';
import Constants from 'expo-constants';
import * as Device from 'expo-device';
import { announcementData, keyword } from './components/globals';


const apiUrl = process.env.EXPO_PUBLIC_API_URL

// thanks to https://github.com/mislav
function timeout(ms, promise: Promise<any>) {
  return new Promise(function(resolve, reject) {
    setTimeout(function() {
      reject(new Error("timeout"))
    }, ms)
    promise.then(resolve, reject)
  })
}

export async function registerForPushNotificationsAsync() {
  let token;

  if (Platform.OS === 'android') {
    Notifications.setNotificationChannelAsync('default', {
      name: 'default',
      importance: Notifications.AndroidImportance.MAX,
      vibrationPattern: [0, 250, 250, 250],
      lightColor: '#FF231F7C',
    });
  }


  if (Device.isDevice) {
    const { status: existingStatus } = await Notifications.getPermissionsAsync();
    let finalStatus = existingStatus;
    if (existingStatus !== 'granted') {
      const { status } = await Notifications.requestPermissionsAsync();
      finalStatus = status;
    }
    if (finalStatus !== 'granted') {
      alert('Failed to get push token for push notification!');
      return;
    }
    token = await Notifications.getExpoPushTokenAsync({
      projectId: Constants.expoConfig.extra.eas.projectId,
    });
    console.log(token);
  } else {
    alert('Must use physical device for Push Notifications');
  }

  return token.data;
}


export async function sendTokenToServer(token) {
  var keyInStorage = await AsyncStorage.getItem("apikey");
  console.log("keyinstorAge " + keyInStorage)
  if(keyInStorage) return keyInStorage;
   
  var myHeaders = new Headers();
  myHeaders.append("Content-Type", "application/json");
  console.log("registering")
  let data = JSON.stringify({token: token,device: Platform.OS });
  console.log(data)
  return  await timeout(5000,fetch(apiUrl + "register", {
    method: "POST",
    headers: myHeaders,
    body:  data,
    redirect: 'follow'
  })).then(async (response: Response) => {

  keyInStorage = await response.text();
  console.log("keyinstorAge2 " + keyInStorage)
  await AsyncStorage.setItem("apikey",keyInStorage)
  return keyInStorage;
}).catch((e) => { console.log(e); return null; });

}



async function GETAuthorized(route: string){

  console.log("Getting " + route)
  const headers = new Headers();
  headers.append("Content-Type","application/json")
  headers.append("X-API-KEY",await AsyncStorage.getItem("apikey"))
  console.log(await AsyncStorage.getItem("apikey"))
  return await timeout(5000,fetch(apiUrl + route, {
    method: "GET",
    headers: headers,
  })).then((response:Response) => {return response.json()});

}
async function POSTAuthorized(route: string, formData: FormData){

  console.log("Posting to " + apiUrl + route   )

  const headers = new Headers();
  headers.append("Content-Type","multipart/form-data")
  headers.append("X-API-KEY",await AsyncStorage.getItem("apikey"))
  console.log(await AsyncStorage.getItem("apikey"))
  return await timeout(5000,fetch(apiUrl + route, {
    method: "POST",
    headers: headers,
    body: formData,
  })).then((response:Response) => response.ok);

}
export async function getKeywords(){
  return  GETAuthorized("getKeywords")
}


export async function getAnnouncements(): Promise<announcementData[]>{
  return  GETAuthorized("getData")
}

export async function editKeywords(kws: keyword[]){
  const data = new FormData();
  let kwss;
  if(kws.length != 0) kwss = kws.map(k => k.word).join(",");
  else kwss = ""
  console.log("EditKeywords: " + kwss)
  data.append("keywords",kwss)
  return POSTAuthorized("editKeywords",data)
}
