import React, { createContext, useContext } from "react";
import { StyleSheet } from "react-native";
import {BottomNavigationProps, Text} from "react-native-paper"
import { SafeAreaView } from "react-native-safe-area-context";
import Ionicons from '@expo/vector-icons/Ionicons';
import { figmaToDeviceRatio } from "./Utils";

export interface announcementData{

  id: number,
  title: string,
  text: string,
  date: string,
  workType: string,
  announcementType: string
  
}
interface UserContextValue{
  keywordState:{
    keywords: keyword[],
    setKeywords: any
  }
  announcementState:{
    announcements: announcementData[],
    setAnnouncements: any
  }
}


export interface keywordChip extends keyword{
  selected: boolean
}

const v: UserContextValue ={keywordState:{  keywords: [],setKeywords: () => {}},announcementState:{  announcements: [],setAnnouncements: () => {}  }}
export const UserContext = createContext(v)

const c: keywordChip[] = [];
export const SelectedChipContext = createContext(c);
export const globals = {
  blue: "#003F7D",
  blueD1: "#003366",
  blueD2: "#002347",
  orange: "#FD7702",
  orangeL1: "#FF8E00",
  background: "#DFE7EE",
  screenOptions: (props: any) => ({
          headerShown: false,
          headerMode: 'none',
          tabBarIcon: ({ focused, color, size }) => {
            let iconName;

            if (props.route.name === 'Home') {
              iconName = focused
                ? 'home'
                : 'home-outline';
            } else if (props.route.name === 'Vodovod') {
              iconName = focused ? 'water' : 'water-outline';
            } else if (props.route.name == "Eps"){
              iconName = focused ? 'flash' : 'flash-outline';
              }

            // You can return any component that you like here!
            return <Ionicons style={{}} name={iconName} size={size} color={color} />;
          },
          tabBarActiveTintColor: '#FD7702',
          tabBarInactiveTintColor: 'white',
          tabBarLabel: ({focused}) => {
            switch (props.route.name){
                case "Home":
                  return focused ? <Text style={styles.tabBarLabelStyle}>Home</Text> : null
                  break;
                case "Vodovod":
                  return focused ? <Text style={styles.tabBarLabelStyle}>Vodovod</Text> : null
                  break;
                case "Eps":
                  return focused ? <Text style={styles.tabBarLabelStyle}>Eps</Text> : null
                  break;
              } 
          },
          tabBarStyle: {
              backgroundColor: "#003F7D",
              borderTopLeftRadius: 10,
              borderTopRightRadius: 10,
              borderLeftWidth: 0.2,
              borderRightWidth: 0.2,
              height:   figmaToDeviceRatio(56,'y'),
              paddingVertical: 5,
              overflow: 'hidden',
            },
          
        } as BottomTabNavigationOptions)
}

export const theme = {
  "colors": {
    "primary": "rgb(38, 94, 167)",
    "onPrimary": "rgb(255, 255, 255)",
    "primaryContainer": "rgb(214, 227, 255)",
    "onPrimaryContainer": "rgb(0, 27, 60)",
    "secondary": "rgb(154, 70, 0)",
    "onSecondary": "rgb(255, 255, 255)",
    "secondaryContainer": "rgb(255, 219, 201)",
    "onSecondaryContainer": "rgb(50, 18, 0)",
    "tertiary": "rgb(0, 102, 138)",
    "onTertiary": "rgb(255, 255, 255)",
    "tertiaryContainer": "rgb(196, 231, 255)",
    "onTertiaryContainer": "rgb(0, 30, 44)",
    "error": "rgb(186, 26, 26)",
    "onError": "rgb(255, 255, 255)",
    "errorContainer": "rgb(255, 218, 214)",
    "onErrorContainer": "rgb(65, 0, 2)",
    "background": "rgb(253, 251, 255)",
    "onBackground": "rgb(26, 28, 30)",
    "surface": "rgb(253, 251, 255)",
    "onSurface": "rgb(26, 28, 30)",
    "surfaceVariant": "rgb(224, 226, 236)",
    "onSurfaceVariant": "rgb(67, 71, 78)",
    "outline": "rgb(116, 119, 127)",
    "outlineVariant": "rgb(196, 198, 207)",
    "shadow": "rgb(0, 0, 0)",
    "scrim": "rgb(0, 0, 0)",
    "inverseSurface": "rgb(47, 48, 51)",
    "inverseOnSurface": "rgb(241, 240, 244)",
    "inversePrimary": "rgb(168, 200, 255)",
    "elevation": {
      "level0": "transparent",
      "level1": "rgb(242, 243, 251)",
      "level2": "rgb(236, 238, 248)",
      "level3": "rgb(229, 234, 245)",
      "level4": "rgb(227, 232, 244)",
      "level5": "rgb(223, 229, 243)"
    },
    "surfaceDisabled": "rgba(26, 28, 30, 0.12)",
    "onSurfaceDisabled": "rgba(26, 28, 30, 0.38)",
    "backdrop": "rgba(45, 48, 56, 0.4)"
  }
}


export const styles = StyleSheet.create({
  bottomBar: {
    backgroundColor: "#003F7D",
    overflow:"hidden",
    position: 'absolute',
  },

  tabBarLabelStyle:{
    fontFamily: "Dosis_700Bold",
    color: globals.orange,
    fontSize: 12,
    marginBottom: figmaToDeviceRatio(7,'y')
  },
});

export interface keyword{
  word: string,
  id: number | string

}

interface VodovodScreenProps extends NavigationInjectedProps {
  keywords: keyword[];
}
