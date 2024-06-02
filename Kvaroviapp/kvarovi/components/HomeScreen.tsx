import { useContext } from 'react';
import { Text, View, StyleSheet } from 'react-native';
import {  figmaToDeviceRatio } from './Utils';
import BluePeople from '../assets/plaviLjudi';
import { UserContext, announcementData, globals, keyword, } from './globals';
import {  TouchableRipple } from 'react-native-paper';
import OrangePeople from '../assets/vodovodLjudi';
import { SafeAreaView, useSafeAreaInsets } from 'react-native-safe-area-context';
import { KeywordsFabModal } from "./keywordsModal";
import { GestureHandlerRootView } from 'react-native-gesture-handler';
import { BottomSheetModalProvider } from "@gorhom/bottom-sheet";
import { useNavigation, useRoute } from '@react-navigation/native';

export const countAnnouncements = (data: announcementData[]) => {
  const predicate = (i,worktype,aType) => data[i].workType == worktype && data[i].announcementType == aType 
  var epsCountCurrent = 0;
  var epsCountPlanned = 0;
  var vodovodCountCurrent = 0;
  var vodovodCountPlanned = 0;
  if(!data) return {epsCountCurrent,epsCountPlanned,vodovodCountCurrent,vodovodCountPlanned};
  for(var i = 0; i < data.length; i++){
    if(predicate(i,"Current","eps")) epsCountCurrent++;
    else if(predicate(i,"Planned","eps")) epsCountPlanned++;
    else if(predicate(i,"Current","vodovod")) vodovodCountCurrent++;
    else if(predicate(i,"Planned","vodovod")) vodovodCountPlanned++;
  }
  return {epsCountCurrent,epsCountPlanned,vodovodCountCurrent,vodovodCountPlanned}
}

export function HomeScreen(props: {}) {
  const navigation = useNavigation<any>();
  const route = useRoute();
  const insets = useSafeAreaInsets();


  const { keywords, setKeywords } = useContext(UserContext).keywordState;
  const { announcements, setAnnouncements } = useContext(UserContext).announcementState;

  let {epsCountCurrent,epsCountPlanned,vodovodCountCurrent,vodovodCountPlanned} = countAnnouncements(announcements)
  console.log("use context home screen " + keywords)


  return (


    
    <GestureHandlerRootView style={{ backgroundColor: globals.background, flex: 1, alignItems: 'center', justifyContent: 'center' }}>
      <BottomSheetModalProvider>

      <SafeAreaView >


          { process.env.NODE_ENV == "development" ?
            <View>
              <Text>API URL: {process.env.EXPO_PUBLIC_API_URL }</Text>
              <Text>Environment: <Text style={{color:"green",fontWeight: "bold"}}>{process.env.NODE_ENV}</Text></Text>
            </View>
              : null
          }
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

              <Text adjustsFontSizeToFit={true} allowFontScaling={false} style={{ fontFamily: 'Dosis_700Bold', fontSize: 14, color: "white" }}>Kvarovi na mreži</Text>
              <View style={{ flexBasis: "100%" }} />
              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 20,  color: globals.orangeL1 }}>{epsCountCurrent}</Text>
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
              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 14,  color: "white" }}>Planirani radovi</Text>
              <View style={{ flexBasis: "100%" }} />
              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 20,  color: globals.orangeL1 }}>{epsCountPlanned}</Text>
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

              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 14,  color: "white" }}>Kvarovi na mreži</Text>
              <View style={{ flexBasis: "100%" }} />
              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 20, color: globals.blueD2 }}>{vodovodCountCurrent}</Text>
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
              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 14,  color: "white" }}>Planirani radovi</Text>
              <View style={{ flexBasis: "100%" }} />
              <Text style={{ fontFamily: 'Dosis_700Bold', fontSize: 20, color: globals.blueD2 }}>{vodovodCountPlanned}</Text>
            </View>
          </TouchableRipple>

        </View>


        <KeywordsFabModal
          wrapperStyle={[styles.fabWrapper, { top: styles.fabWrapper.top - insets.top }]} color={globals.orange}
          FABstyle={styles.fab}
          onModalChange={() => { }}

        />
      </SafeAreaView>

      </BottomSheetModalProvider>

    </GestureHandlerRootView>

  );
}

//<Text style={{fontFamily: "Dosis_700Bold",fontSize:20}}>Eleketroprivreda Srbije</Text>
const styles = StyleSheet.create({
  mainCard: {
    width: figmaToDeviceRatio(315, 'x'),
    height: "auto",
    backgroundColor: "white",
    borderRadius: 12,
    display: "flex",
    justifyContent: "center",
    flexWrap: "wrap",
    flexDirection: "row",
    padding: 6,
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
    alignItems: 'center',
    flexWrap: 'wrap',
    width: figmaToDeviceRatio(139, 'x'),
    maxHeight: "auto",
    borderRadius: 12,
    paddingHorizontal: 8,
    paddingVertical: figmaToDeviceRatio(10,'y'),
  
  },

  informationBoxWrapper: {
    width: figmaToDeviceRatio(139, 'x'),
    borderRadius: 12,
    marginBottom: 10,
  
  },

  fab: {
    backgroundColor: globals.blue,
  },
  fabWrapper: {
    position: 'absolute',
  
    right: 5,
    top: figmaToDeviceRatio(740, 'y')

  }
});


