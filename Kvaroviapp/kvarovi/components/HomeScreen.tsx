import { useState, useEffect, useRef, forwardRef } from 'react';
import { Text, View, Dimensions, Image, Platform, StyleSheet } from 'react-native';
import { createAnimatedFunctionComponent, figmaToDeviceRatio } from './Utils';
import { useFonts, Dosis_700Bold } from '@expo-google-fonts/dosis';
import BluePeople from '../assets/plaviLjudi';
import { globals, keyword } from './globals';
import { FAB, Portal, Surface, TouchableRipple } from 'react-native-paper';
import OrangePeople from '../assets/vodovodLjudi';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { KeywordsFabModal } from './keywordsModal';
import { GestureHandlerRootView } from 'react-native-gesture-handler';
import { BottomSheetModalProvider } from "@gorhom/bottom-sheet";
import { ScreenProps } from 'react-native-screens';
import { useNavigation, useRoute } from '@react-navigation/native';
import Animated, { useAnimatedProps, useSharedValue, withRepeat, withSequence, withTiming } from 'react-native-reanimated';
import React from 'react';
import { Path } from 'react-native-svg';
export function HomeScreen(props: { keywords: keyword[] }) {
  const navigation = useNavigation<any>();
  const route = useRoute();
  const insets = useSafeAreaInsets();;
  useEffect(() => {
  })

  return (

    <GestureHandlerRootView style={{ backgroundColor: globals.background, flex: 1, alignItems: 'center', justifyContent: 'center' }}>
      <BottomSheetModalProvider>
        <View style={styles.mainCard}>

          <Text style={[styles.cardHeader, { color: globals.blueD1 }]}>Eleketroprivreda Srbije</Text>

          <View style={[styles.peopleContainer, { backgroundColor: globals.blue }]}>
            {
              // lightbulb color  #ffcf03
              // lightbulb color outer #ffb502
            }
            <BluePeople
              innerLightColor="#ffcf03" outerLightColor="#ffb502"
              style={{ width: figmaToDeviceRatio(288, 'x'), height: figmaToDeviceRatio(177, 'y') }} />

          </View>
          <TouchableRipple
            style={[styles.informationBoxWrapper, { marginRight: figmaToDeviceRatio(9, 'x') }]}
            onPressOut={() => { navigation.navigate("Eps", { segmentSelection: "trenutni" }) }}
            borderless={true}
            onPress={() => { }}
            rippleColor={"rgba(255, 255, 255, .33)"}
          >
            <View
              style={[styles.informationBox, { backgroundColor: globals.blue, }]}>

              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 14, lineHeight: 17, color: "white" }}>Kvarovi na mreži</Text>
              <View style={{ flexBasis: "100%" }} />
              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 20, lineHeight: 35, color: globals.orangeL1 }}>1</Text>
            </View>

          </TouchableRipple>

          <TouchableRipple
            style={[styles.informationBoxWrapper]}

            onPressOut={() => { navigation.navigate("Eps", { segmentSelection: "planirani" }) }}
            borderless={true}
            onPress={() => { }}
            rippleColor={"rgba(255, 255, 255, .33)"}
          >
            <View style={[styles.informationBox, { backgroundColor: globals.blue }]}>
              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 14, lineHeight: 17, color: "white" }}>Planirani radovi</Text>
              <View style={{ flexBasis: "100%" }} />
              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 20, lineHeight: 35, color: globals.orangeL1 }}>1</Text>
            </View>
          </TouchableRipple>
        </View>

        <View style={[styles.mainCard, { marginBottom: figmaToDeviceRatio(94, 'y') }]}>

          <Text style={[styles.cardHeader, { color: globals.orange }]}>Vodovod Srbije</Text>
          <OrangePeople style={[styles.peopleContainer, { backgroundColor: globals.orange }]} />

          <TouchableRipple
            style={[styles.informationBoxWrapper, { marginRight: figmaToDeviceRatio(9, 'x') }]}
            onPressOut={() => { navigation.navigate("Vodovod", { segmentSelection: "trenutni" }) }}
            borderless={true}
            onPress={() => { }}
            rippleColor={"rgba(255, 255, 255, .33)"}
          >
            <View style={[styles.informationBox, { backgroundColor: globals.orange, }]}>

              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 14, lineHeight: 17, color: "white" }}>Kvarovi na mreži</Text>
              <View style={{ flexBasis: "100%" }} />
              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 20, lineHeight: 35, color: globals.blueD2 }}>1</Text>
            </View>
          </TouchableRipple>

          <TouchableRipple
            style={[styles.informationBoxWrapper]}
            onPressOut={() => { navigation.navigate("Vodovod", { segmentSelection: "planirani" }) }}
            borderless={true}
            onPress={() => { }}
            rippleColor={"rgba(255, 255, 255, .33)"}
          >
            <View style={[styles.informationBox, { backgroundColor: globals.orange }]}>
              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 14, lineHeight: 17, color: "white" }}>Planirani radovi</Text>
              <View style={{ flexBasis: "100%" }} />
              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 20, lineHeight: 35, color: globals.blueD2 }}>1</Text>
            </View>
          </TouchableRipple>

        </View>

        <KeywordsFabModal
          keywords={props.keywords}
          wrapperStyle={[styles.fabWrapper, { top: styles.fabWrapper.top - insets.top }]} color={globals.orange}
          FABstyle={styles.fab}
          onModalChange={() => { }}
        />
      </BottomSheetModalProvider>
    </GestureHandlerRootView>

  );
}

//<Text style={{fontFamily: "Dosis_700Bold",fontSize:20}}>Eleketroprivreda Srbije</Text>
const styles = StyleSheet.create({
  mainCard: {
    width: figmaToDeviceRatio(315, 'x'),
    height: figmaToDeviceRatio(313, 'y'),
    backgroundColor: "white",
    borderRadius: 12,
    display: "flex",
    justifyContent: "center",
    flexWrap: "wrap",
    flexDirection: "row",
    padding: figmaToDeviceRatio(6, 'y'),
    marginBottom: figmaToDeviceRatio(9, 'y')
  },
  cardHeader: {
    fontFamily: "Dosis_700Bold",
    fontSize: 20

  },
  peopleContainer: {
    width: figmaToDeviceRatio(288, 'x'),
    height: figmaToDeviceRatio(188, 'y'),
    borderRadius: 12,
    marginBottom: figmaToDeviceRatio(10, 'y')
  },
  informationBox: {
    display: 'flex',
    flexDirection: 'row',
    justifyContent: 'center',
    flexWrap: 'wrap',
    width: figmaToDeviceRatio(139, 'x'),
    height: figmaToDeviceRatio(67.15, 'y'),
    borderRadius: 12,
    padding: 8,
  },
  informationBoxWrapper: {
    width: figmaToDeviceRatio(139, 'x'),
    height: figmaToDeviceRatio(67.15, 'y'),
    borderRadius: 12
  },

  fab: {
    backgroundColor: globals.blue,
  },
  fabWrapper: {
    position: 'absolute',
    margin: 16,
    right: 0,
    top: figmaToDeviceRatio(690, 'y')

  }
});

