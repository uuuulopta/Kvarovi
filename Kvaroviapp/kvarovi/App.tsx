import { useState, useEffect, useRef, useCallback, useMemo} from 'react';
import { Platform, StatusBar, ToastAndroid} from 'react-native';
import { PaperProvider, Snackbar} from 'react-native-paper';
import * as Device from 'expo-device';
import * as Notifications from 'expo-notifications';
import Constants from 'expo-constants';
import { NavigationContainer} from '@react-navigation/native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createNativeStackNavigator } from '@react-navigation/native-stack'
import { HomeScreen } from './components/HomeScreen';
import { useFonts, Dosis_700Bold, Dosis_500Medium,  Dosis_600SemiBold, } from '@expo-google-fonts/dosis';
import { Dongle_700Bold } from '@expo-google-fonts/dongle';
import * as SplashScreen from 'expo-splash-screen';
import { globals, theme, keyword, UserContext, announcementData} from './components/globals';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import { ListingsScreen } from './components/VodovodScreen';
import {getKeywords,getAnnouncements,sendTokenToServer,registerForPushNotificationsAsync,  editKeywords} from './RequestHandlers'
import AsyncStorage from '@react-native-async-storage/async-storage';

Notifications.setNotificationHandler({
  handleNotification: async () => ({
    shouldShowAlert: true,
    shouldPlaySound: false,
    shouldSetBadge: false,
  }),
});


// Can use this function below or use Expo's Push Notification Tool from: https://expo.dev/notifications


const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();

// TODO add splashscreen disable autohide? 
SplashScreen.preventAutoHideAsync()
export default function App() {

  var a = useRef(true)
  a.current = false;
  const [notification, setNotification] = useState<Notifications.Notification | undefined>(undefined);
  const notificationListener = useRef<Notifications.Subscription>();
  const responseListener = useRef<Notifications.Subscription>();


  const [snackbar, setSnackbar] = useState();
  const [keywords, setKeywords] = useState<keyword[]>([]);
  const [announcements, setAnnouncements] = useState<announcementData[]>([]);
  const [dataGathered,setDataGathered] = useState(false);
  const value = {  keywordState: { keywords, setKeywords }, announcementState: {announcements, setAnnouncements} };

  const getData = async() => {

    var kw =  getKeywords();
    var announcements = getAnnouncements(); 
    const [kwRes, announcementsRes] = await Promise.all([kw, announcements]);
    console.log(announcementsRes)
    setKeywords([...kwRes])
    setAnnouncements([...announcementsRes])
  }
    

 useEffect(() => {
 

    console.log("rendered!")

    if(dataGathered){

    }
    else{
      AsyncStorage.clear();
      registerForPushNotificationsAsync().then(token => {
         sendTokenToServer(token).then(async () => await getData() )
          .catch((e) => {
          if(Device.osName == "Android") ToastAndroid.show(e.toString(),3000)
            setDataGathered(true);
            SplashScreen.hideAsync();
          })
          .finally(() => { 
            setDataGathered(true);
            SplashScreen.hideAsync()
          });
      })
    }


    notificationListener.current = Notifications.addNotificationReceivedListener(notification => {
      setNotification(notification);
      getData();
    });

    responseListener.current = Notifications.addNotificationResponseReceivedListener(response => {

      getData();
    });

    return () => {
      Notifications.removeNotificationSubscription(notificationListener.current);
      Notifications.removeNotificationSubscription(responseListener.current);
    };
  },[]);


  let [fontsLoaded, fontError] = useFonts({
    Dosis_700Bold,
    Dosis_600SemiBold,
    Dosis_500Medium,
    Dongle_700Bold

  });
  const onLayoutRootView = useCallback(async () => {
    console.log(`Onlayout fonts: ${fontsLoaded} data: ${dataGathered}`)
    if (( fontsLoaded || fontError ) && dataGathered) {
      SplashScreen.hideAsync()
    }

  }, [fontsLoaded, fontError,dataGathered]);

  if (!fontsLoaded && !fontError) {
    return null;
  }


  


  const VodovodScreen = () => {

    return <ListingsScreen institution='vodovod'/>
  }

  const EpsScreen = () => {

    return <ListingsScreen institution='eps'/>
  }
  const BottomTab = () => {

    return
  }



  return (

    <PaperProvider theme={theme}>

      <SafeAreaProvider style={{ backgroundColor: globals.background }}>
        <NavigationContainer onReady={onLayoutRootView}>
  <StatusBar translucent={true} backgroundColor="transparent" barStyle="dark-content" />
 <UserContext.Provider value={value}>
          <Tab.Navigator
            screenOptions={globals.screenOptions}
          >

            <Tab.Screen name="Home"  children={() =><HomeScreen />}/>
            <Tab.Screen name="Vodovod" children={() => <VodovodScreen />} />
            <Tab.Screen name="Eps" children={() => <EpsScreen />}  />
          </Tab.Navigator>
</UserContext.Provider>
        </NavigationContainer>
      </SafeAreaProvider>
    </PaperProvider >
  );
}

