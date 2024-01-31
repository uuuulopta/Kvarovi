import { useState, useEffect, useRef, useCallback } from 'react';
import { Text, View, Dimensions, Platform, StyleSheet } from 'react-native';
import { PaperProvider, useTheme } from 'react-native-paper';
import * as Device from 'expo-device';
import * as Notifications from 'expo-notifications';
import Constants from 'expo-constants';
import { NavigationContainer } from '@react-navigation/native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createNativeStackNavigator } from '@react-navigation/native-stack'
import { figmaToDeviceRatio } from './components/Utils';
import { HomeScreen } from './components/HomeScreen';
import Ionicons from '@expo/vector-icons/Ionicons';
import { useFonts, Dosis_700Bold, Dosis_300Light, Dosis_500Medium, Dosis_400Regular, Dosis_600SemiBold, Dosis_800ExtraBold, Dosis_200ExtraLight } from '@expo-google-fonts/dosis';
import { Dongle_700Bold } from '@expo-google-fonts/dongle';
import * as SplashScreen from 'expo-splash-screen';
import { globals, theme, keyword } from './components/globals';
import { SafeAreaProvider, SafeAreaView } from 'react-native-safe-area-context';
import BluePeople from './assets/plaviLjudi';
import { ListingsScreen } from './components/VodovodScreen';

Notifications.setNotificationHandler({
  handleNotification: async () => ({
    shouldShowAlert: true,
    shouldPlaySound: false,
    shouldSetBadge: false,
  }),
});


// Can use this function below or use Expo's Push Notification Tool from: https://expo.dev/notifications

async function registerForPushNotificationsAsync() {
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


async function sendTokenToServer(token) {
  let formdata = new FormData;
  formdata.append("token", token);
  formdata.append("device", Platform.OS);
  await fetch("http://192.168.0.3:5050/register", {
    method: "POST",
    body: formdata
  });

}


const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();

SplashScreen.preventAutoHideAsync();
export default function App() {



  const [notification, setNotification] = useState<Notifications.Notification | undefined>(undefined);
  const notificationListener = useRef<Notifications.Subscription>();
  const responseListener = useRef<Notifications.Subscription>();



  const kw1: keyword = { word: "Ulica br 1", id: 1 }
  const kw2: keyword = { word: "Ulica br 2", id: 2 }
  const kw3: keyword = { word: "Ulica br 3", id: 3 }
  const kw4: keyword = { word: "lorem", id: 4 }
  const kws: keyword[] = [kw1, kw2, kw3, kw4];
  const [keywords, setKeywords] = useState<keyword[]>(kws);

  let [fontsLoaded, fontError] = useFonts({
    Dosis_700Bold,
    Dosis_600SemiBold,
    Dosis_500Medium,
    Dongle_700Bold

  });
  const onLayoutRootView = useCallback(async () => {
    if (fontsLoaded || fontError) {
      await SplashScreen.hideAsync();
      console.log(fontsLoaded)
    }

  }, [fontsLoaded, fontError]);

  if (!fontsLoaded && !fontError) {
    return null;
  }


  // useEffect(() => {
  //registerForPushNotificationsAsync().then(token => {
  //setExpoPushToken(token)
  //sendTokenToServer(token);
  //});

  //notificationListener.current = Notifications.addNotificationReceivedListener(notification => {
  //  setNotification(notification);
  //});

  //responseListener.current = Notifications.addNotificationResponseReceivedListener(response => {
  //});

  //return () => {
  //  Notifications.removeNotificationSubscription(notificationListener.current);
  //  Notifications.removeNotificationSubscription(responseListener.current);
  //};
  //}, []);



  const VodovodScreen = (props: {keywords: keyword[]}) => {

    return <ListingsScreen keywords={props.keywords} institution='vodovod'/>
  }

  const EpsScreen = (props: {keywords: keyword[]}) => {

    return <ListingsScreen keywords={props.keywords} institution='eps'/>
  }
  const BottomTab = () => {

    return <Tab.Navigator
      screenOptions={globals.screenOptions}
    >

      <Tab.Screen name="Home" children={() => <HomeScreen keywords={keywords} />} />
      <Tab.Screen name="Vodovod" children={() => <VodovodScreen keywords={keywords} />} />
      <Tab.Screen name="Eps" children={() => <EpsScreen keywords={keywords}/>}  />
    </Tab.Navigator>
  }



  return (

    <PaperProvider theme={theme}>

      <SafeAreaProvider style={{ backgroundColor: globals.background }}>
        <NavigationContainer onReady={onLayoutRootView}>
          {BottomTab()}
        </NavigationContainer>
      </SafeAreaProvider>
    </PaperProvider >
  );
}

