import { useState, useEffect, useRef, useContext, useMemo } from 'react';
import { Text, View, Dimensions,Image, Platform, StyleSheet, LayoutAnimation } from 'react-native';
import { figmaToDeviceRatio } from './Utils';
import { useFonts,Dosis_700Bold } from '@expo-google-fonts/dosis';
import BluePeople from '../assets/plaviLjudi';
import { SelectedChipContext, UserContext, globals, keywordChip } from './globals';
import { FAB, List, Provider } from 'react-native-paper';
import OrangePeople from '../assets/vodovodLjudi';
import { Dongle_700Bold } from '@expo-google-fonts/dongle';
import { keyword } from './globals';
import ParsedText from 'react-native-parsed-text';
import { ScrollView } from 'react-native';

export interface kvarAccordionProps{
  date: string,
  title: string,
  titleShort: string,
  text: string,
  id: string | number,
  expanded: boolean,

}




export function KvaroviAccordion(props: {accordionProps:  kvarAccordionProps,selectedKeywords }){

  



  const {keywords,setKeywords} = useContext(UserContext).keywordState;
  

  return <List.Accordion 
       
    style={[ styles.kvarStyle,props.accordionProps.expanded ? {borderBottomLeftRadius:0,borderBottomRightRadius:0} : {} ]}
    theme={{ colors: {background:globals.background} }}
    title={props.accordionProps.titleShort } 
    left={() => <DateHeader date={props.accordionProps.date}/>}
    titleStyle={{fontFamily:"Dosis_700Bold",fontSize:20}}
    id={props.accordionProps.id}
    key={props.accordionProps.id}
    expanded={props.accordionProps.expanded}
    
    
    
  > 
    <View style={[ styles.accordionTextWrapper]}>

      <ScrollView >
      <Text style={[{fontFamily:"Dosis_600SemiBold",fontSize:20,marginBottom:10,}]}>{props.accordionProps.title}</Text>
      <ParsedText
        style={[styles.accordionText,{lineHeight:20}]}
        parse= { [
          {pattern: new RegExp(keywords.map(kw =>kw.word).join("|"),"gmi" ), style: {color:"red"} }
        ] }
      >{props.accordionProps.text}</ParsedText>
      </ScrollView>
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
    height: "auto",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",  
    marginTop: 6,
  },

  accordionTextWrapper: {

    borderRadius: 12,
    backgroundColor: "white",
    width: figmaToDeviceRatio(331,'x'),
    maxHeight: figmaToDeviceRatio(300,'y'),
    display: "flex",
    marginTop: 0,
    fontFamily:"Dosis_500Medium",
    borderTopLeftRadius: 0,
    borderTopRightRadius: 0,
    paddingBottom: 10,
  },
  accordionText:{
    fontSize:16,
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
