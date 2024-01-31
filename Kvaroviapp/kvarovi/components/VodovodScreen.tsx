import { useState, useEffect, useRef, useMemo } from 'react';
import { Text, View, Dimensions,Image, Platform, StyleSheet, LayoutAnimation } from 'react-native';
import { figmaToDeviceRatio } from './Utils';
import { useFonts,Dosis_700Bold } from '@expo-google-fonts/dosis';
import BluePeople from '../assets/plaviLjudi';
import { globals } from './globals';
import { FAB, List, SegmentedButtons,Chip } from 'react-native-paper';
import OrangePeople from '../assets/vodovodLjudi';
import { Dongle_700Bold } from '@expo-google-fonts/dongle';
import { keyword } from './globals';
import { KvaroviAccordion, kvarAccordionProps } from './KvaroviAccordion';
import { useNavigation, useRoute } from '@react-navigation/native';
export function ListingsScreen(props: {keywords: keyword[],institution: string}){



// Assuming your navigation prop is of type NavigationInjectedProps

  if(props.institution == "eps") return <Text>Evo ti ga eps</Text>
  let navigation = useNavigation();

  let route = useRoute();
  const returnKeywords = () => props.keywords;
  var lorem = "Lorem ipsum dolor sit amet, officia excepteur ex fugiat reprehenderit enim labore culpa sint ad nisi Lorem pariatur mollit ex esse exercitation amet. Nisi anim cupidatat excepteur officia. Reprehenderit nostrud nostrud ipsum Lorem est aliquip amet voluptate voluptate dolor minim nulla est proident. Nostrud officia pariatur ut officia. Sit irure elit esse ea nulla sunt ex occaecat reprehenderit commodo officia dolor Lorem duis laboris cupidatat officia voluptate. Culpa proident adipisicing id nulla nisi laboris ex in Lorem sunt duis officia eiusmod. Aliqua reprehenderit commodo ex non excepteur duis sunt velit enim. Voluptate laboris sint cupidatat ullamco ut ea consectetur et est culpa et culpa duis."
  let kv1 = {date:"1.1.2024",titleShort:"Kratki Naslov",title:"Dugacki naslov",text:lorem,id:"1",expanded:false,returnKeywords:returnKeywords}
  let kv2 = {date:"1.1.2024",titleShort:"Kratki Naslov 2",title:"Dugacki naslov",text:lorem,id:"2",expanded:false,returnKeywords:returnKeywords}
  
  const [chipSelection,setChipSelection] = useState<keywordChip[]>( props.keywords.map( (k:any) =>{ k.selected=true; return k; }))
  const [expandedId,setExpandedId] = useState<string>()
  
  const [segmentSelection, setSegmentSelection] = useState<"trenutni" | "planirani">("trenutni");

  let params = route.params as any
useMemo(()=>{

  if(params && params.segmentSelection) setSegmentSelection(params.segmentSelection)
}, [params])
  const [kvarovi,setKvarovi] = useState<kvarAccordionProps[]>([kv1,kv2]);
  const [planirani,setPlanirani] = useState<kvarAccordionProps[]>([
    {date:"1.1.2024",titleShort:"Planirani radovi",title:"Dugacki naslov",text:lorem,id:"4",expanded:false,returnKeywords:returnKeywords}
  ]
)
  console.log("SEGMENT##" +  segmentSelection)
  function handleChange(id){
    if(expandedId != id) setExpandedId(id);
    else setExpandedId('')
    
  }

  function handleChipSelect(id){
    const newChips = chipSelection.map((c) => {
    const newC = Object.assign({},c)
    if(newC.id == id) newC.selected = !(newC.selected) 
    return newC;
   }) 
   setChipSelection(newChips)
     
  }

  function getSelectedAccordions(){
    let arr;
    if(segmentSelection == "planirani") arr = planirani
    else arr = kvarovi
    console.log(planirani[0].title)
    return arr.length != 0 ? arr.map( (k) =>
        <KvaroviAccordion expanded={k.expanded} key={k.id} id={k.id} title={k.title} date={k.date} titleShort={k.titleShort} text={k.text} returnKeywords={returnKeywords}/>)
      : <Text style={{fontFamily: "Dosis_700Bold",fontSize: 16,color: "#4E4953"}}>Trenutno nema rezultata za vaše ključne reči!</Text>


  }

  return <View  style={{width:"100%",height:"100%", backgroundColor:globals.background,display:"flex",flexDirection: "row",justifyContent:"center",flexWrap:"wrap"}}>
    <View style={{display:"flex",justifyContent: "flex-start",width:figmaToDeviceRatio(311,'x')}}>
      <Text style={{fontFamily: "Dosis_700Bold",fontSize: 16,color: "#4E4953"}}>Kvarovi Vodovod</Text>
    </View>
    <SegmentedButtons onValueChange={setSegmentSelection as unknown as any} value={segmentSelection}
      buttons= { [ {checkedColor:globals.orange,value:"trenutni", label:"Trenutni",labelStyle: styles.segmentTexStyle, icon: segmentSelection == "trenutni" ? "alert" : "alert-outline" },
      {checkedColor:globals.blue,value:"planirani",label:"Planirani",labelStyle: styles.segmentTexStyle,icon: segmentSelection == "planirani" ? "clipboard-list" : "clipboard-list-outline"} ] }
      style={styles.segmentStyle}
    />
    <View style={{display:"flex",gap:2, flexWrap: "wrap",flexDirection: "row",justifyContent: "center",marginTop: figmaToDeviceRatio(5,'y')}}> 
    {chipSelection.map((c) => <Chip onPress={() => handleChipSelect(c.id)} selected={ c.selected } key={c.id}>{c.word}</Chip>)}
    </View>
    <View style={{flexBasis: "100%"}}/>
    <List.AccordionGroup expandedId={expandedId} onAccordionPress={handleChange}>
     
      
      {useMemo(() => getSelectedAccordions(),[segmentSelection])}

    </List.AccordionGroup>
  </View>
}

const styles = StyleSheet.create({
  segmentStyle:
  {
    borderRadius: 20,
    backgroundColor: "#ffffffaa",
    width: figmaToDeviceRatio(331,'x'),
    height: figmaToDeviceRatio(39,'y'),
    overflow: "hidden",
  },
  segmentTexStyle:{
    fontSize: 17,
    fontFamily: "Dosis_600SemiBold"
  }
})

interface keywordChip extends keyword{
  selected: boolean
}



