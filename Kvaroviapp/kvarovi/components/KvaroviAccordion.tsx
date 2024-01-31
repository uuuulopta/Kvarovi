import { useState, useEffect, useRef } from 'react';
import { Text, View, Dimensions,Image, Platform, StyleSheet, LayoutAnimation } from 'react-native';
import { figmaToDeviceRatio } from './Utils';
import { useFonts,Dosis_700Bold } from '@expo-google-fonts/dosis';
import BluePeople from '../assets/plaviLjudi';
import { globals } from './globals';
import { FAB, List } from 'react-native-paper';
import OrangePeople from '../assets/vodovodLjudi';
import { Dongle_700Bold } from '@expo-google-fonts/dongle';
import { keyword } from './globals';
import ParsedText from 'react-native-parsed-text';

export interface kvarAccordionProps{
  date: string,
  title: string,
  titleShort: string,
  text: string,
  id: string,
  expanded: boolean,
  returnKeywords: () => keyword[]

}


export function KvaroviAccordion(props: kvarAccordionProps){

  
  useEffect(() => {},[props.expanded])
  return <List.Accordion 
    
    
    style={[ styles.kvarStyle,props.expanded ? {borderBottomLeftRadius:0,borderBottomRightRadius:0} : {} ]}
    theme={{ colors: {background:globals.background} }}
    title={props.titleShort } 
    left={() => <DateHeader date={props.date}/>}
    titleStyle={{fontFamily:"Dosis_700Bold",fontSize:20}}
    id={props.id}
    key={props.id}
    expanded={props.expanded}
    
    
    
  > 
    <View style={[ styles.kvarStyle,styles.accordionTextWrapper]}>

      <Text style={[styles.accordionText,{fontFamily:"Dosis_600SemiBold",fontSize:20,}]}>{"Neki dugackiiiiiiii naslov"}</Text>
      <ParsedText
        style={[styles.accordionText,{lineHeight:20}]}
        parse= { [
          {pattern: new RegExp(props.returnKeywords().map(kw =>kw.word).join("|"),"gmi" ),onTextLayout: (e) => {console.log(`Matched!`)}, style: {color:"red"},  }
        ] }
      >{props.text}</ParsedText>
    </View>
  </List.Accordion>
}

function DateHeader(props:{date: string}){
  return <View style={styles.dateHeader}>

    <Text style={{fontFamily:"Dongle_700Bold",color:"white",fontSize:28}}>{props.date}</Text>
  </View>

}
const styles = StyleSheet.create({
  
  kvarStyle: {
    borderRadius: 12,
    backgroundColor: "white",
    width: figmaToDeviceRatio(331,'x'),
    height: figmaToDeviceRatio(39,'y'),
    display: "flex",
    justifyContent: "center",
    alignItems: "center",  
    marginTop: 6,
  },

  accordionTextWrapper: {
    
    height: "auto",
    fontFamily:"Dosis_500Medium",
    marginTop:0,
    borderTopLeftRadius: 0,
    borderTopRightRadius: 0,
  },
  accordionText:{

    fontSize:16,
    width: figmaToDeviceRatio(331,'x'),
    marginRight: figmaToDeviceRatio(33,'x'),
    padding:5
  },
  kvarStyleItem: {
    borderBottomRightRadius: 12,
    borderBottomLeftRadius: 12,
    
    backgroundColor: "white",
    width: figmaToDeviceRatio(331,'x'),
    marginBottom: figmaToDeviceRatio(16,'y'),
    display: "flex",
    justifyContent: "center",
    alignItems: "center"  
  },
  dateHeader: {
    width: figmaToDeviceRatio(88,'x'),
    height: figmaToDeviceRatio(33,'y'),
    backgroundColor: globals.blue,
    borderRadius: 12,
    display: "flex",
    justifyContent: "center",
    alignItems: "center",  
    marginTop: figmaToDeviceRatio(3,'y'),
    marginBottom: figmaToDeviceRatio(3,'y'),
    marginLeft: figmaToDeviceRatio(3,'x'),
    marginRight: figmaToDeviceRatio(3,'x'),

  },
  
  
})
